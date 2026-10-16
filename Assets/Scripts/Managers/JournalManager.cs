using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JournalManager : MonoBehaviour
{
    public PlayerInteractor interactor;

    [Header("Layout")]
    public RectTransform panel;
    public RectTransform viewport;
    public RectTransform content;

    [Header("Open / Close")]
    [Tooltip("How far below its normal spot the panel sits while hidden.")]
    public float slideDistance = 1200f;
    public float slideTime = 0.35f;

    [Header("Edge Scroll")]
    [Tooltip("Part of the screen height at the top/bottom that scrolls (0.1 = 10%).")]
    public float edgeSize = 0.1f;
    public float scrollSpeed = 800f;
    [Tooltip("How far past the page's top/bottom edge you can scroll, as a part of the viewport height (0.1 = 10%).")]
    public float edgePadding = 0.1f;

    [Header("New Clues")]
    public float scrollToClueTime = 0.5f;
    public float fadeTime = 0.6f;
    public float pauseAfterClue = 0.3f;

    readonly List<GameObject> queue = new List<GameObject>();
    Vector2 shownPos;
    bool canScroll;

    void Awake()
    {
        shownPos = panel.anchoredPosition;
        panel.anchoredPosition = shownPos + Vector2.down * slideDistance;
        panel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!interactor.Busy && JournalPressed())
            interactor.RunSequence(Journal());

        if (canScroll) EdgeScroll();
    }

    public void Unlock(GameObject clue)
    {
        if (clue.activeSelf || queue.Contains(clue)) return;
        queue.Add(clue);
    }

    IEnumerator Journal()
    {
        bool oldCursor = Cursor.visible;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        panel.gameObject.SetActive(true);
        yield return Slide(shownPos + Vector2.down * slideDistance, shownPos);

        while (queue.Count > 0)
        {
            GameObject clue = queue[0];
            queue.RemoveAt(0);
            yield return Reveal(clue);
        }

        canScroll = true;
        do yield return null;
        while (!JournalPressed());
        canScroll = false;

        yield return Slide(shownPos, shownPos + Vector2.down * slideDistance);
        panel.gameObject.SetActive(false);
        Cursor.visible = oldCursor;
    }

    IEnumerator Reveal(GameObject clue)
    {
        CanvasGroup group = clue.GetComponent<CanvasGroup>();
        if (group == null) group = clue.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        clue.SetActive(true);
        Canvas.ForceUpdateCanvases();

        RectTransform rect = (RectTransform)clue.transform;
        Vector3 clueCenter = viewport.InverseTransformPoint(rect.TransformPoint(rect.rect.center));
        float target = ClampScroll(content.anchoredPosition.y - (clueCenter.y - viewport.rect.center.y));
        yield return ScrollTo(target);

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            group.alpha = t / fadeTime;
            yield return null;
        }
        group.alpha = 1f;

        yield return new WaitForSeconds(pauseAfterClue);
    }

    IEnumerator ScrollTo(float y)
    {
        float start = content.anchoredPosition.y;
        for (float t = 0f; t < scrollToClueTime; t += Time.deltaTime)
        {
            SetScroll(Mathf.SmoothStep(start, y, t / scrollToClueTime));
            yield return null;
        }
        SetScroll(y);
    }

    IEnumerator Slide(Vector2 from, Vector2 to)
    {
        for (float t = 0f; t < slideTime; t += Time.deltaTime)
        {
            panel.anchoredPosition = Vector2.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / slideTime));
            yield return null;
        }
        panel.anchoredPosition = to;
    }

    void EdgeScroll()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        float y = mouse.position.ReadValue().y / Screen.height;
        float dir = 0f;
        if (y > 1f - edgeSize) dir = -1f;
        else if (y < edgeSize) dir = 1f;

        SetScroll(content.anchoredPosition.y + dir * scrollSpeed * Time.deltaTime);
    }

    void SetScroll(float y)
    {
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, ClampScroll(y));
    }

    float ClampScroll(float y)
    {
        Bounds page = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, content);
        Rect view = viewport.rect;
        float pad = edgePadding * view.height;
        float current = content.anchoredPosition.y;

        float min = current + (view.yMax - pad) - page.max.y;
        float max = current + (view.yMin + pad) - page.min.y;
        if (max < min) max = min;

        return Mathf.Clamp(y, min, max);
    }

    static bool JournalPressed()
    {
        Keyboard kb = Keyboard.current;
        return kb != null && kb.sKey.wasPressedThisFrame;
    }
}