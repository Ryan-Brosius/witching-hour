using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Pages sit side by side as direct children of Content. To add a page, duplicate one and place it to the right.
public class JournalManager : MonoBehaviour
{
    public PlayerInteractor interactor;

    [Header("Layout")]
    public RectTransform panel;
    public RectTransform viewport;
    [Tooltip("Holds the pages. Each direct child is one page.")]
    public RectTransform content;

    [Header("Open / Close")]
    [Tooltip("How far below its normal spot the panel sits while hidden.")]
    public float slideDistance = 1200f;
    public float slideTime = 0.35f;

    [Header("Edge Scroll")]
    [Tooltip("Part of the screen width at the left/right that scrolls (0.1 = 10%).")]
    public float edgeSize = 0.1f;
    public float scrollSpeed = 1500f;
    [Tooltip("How far past the first/last page you can scroll, as a part of the viewport width (0.1 = 10%).")]
    public float edgePadding = 0.05f;

    [Header("New Clues")]
    [Tooltip("Time to slide over to the clue's page.")]
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

        // Clues from older runs :)
        foreach (Transform t in content.GetComponentsInChildren<Transform>(true))
            if (PlayerPrefs.GetInt(SaveKey(t.gameObject)) == 1) t.gameObject.SetActive(true);
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

        PlayerPrefs.SetInt(SaveKey(clue), 1);
        PlayerPrefs.Save();
    }

    static string SaveKey(GameObject clue) => "Journal_" + clue.name;

    [ContextMenu("Clear Saved Clues")]
    void ClearSave()
    {
        foreach (Transform t in content.GetComponentsInChildren<Transform>(true))
            PlayerPrefs.DeleteKey(SaveKey(t.gameObject));
        PlayerPrefs.Save();
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

        // Center the clue's page in the viewport.
        RectTransform page = PageOf(clue);
        Vector3 pageCenter = viewport.InverseTransformPoint(page.TransformPoint(page.rect.center));
        float target = ClampScroll(content.anchoredPosition.x - (pageCenter.x - viewport.rect.center.x));
        yield return ScrollTo(target);

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            group.alpha = t / fadeTime;
            yield return null;
        }
        group.alpha = 1f;

        yield return new WaitForSeconds(pauseAfterClue);
    }

    // Walks up from the clue to the page it lives on (the direct child of Content).
    RectTransform PageOf(GameObject clue)
    {
        Transform t = clue.transform;
        while (t.parent != null && t.parent != content) t = t.parent;
        return (RectTransform)t;
    }

    IEnumerator ScrollTo(float x)
    {
        float start = content.anchoredPosition.x;
        if (Mathf.Abs(start - x) < 1f) yield break; // already on this page

        for (float t = 0f; t < scrollToClueTime; t += Time.deltaTime)
        {
            SetScroll(Mathf.SmoothStep(start, x, t / scrollToClueTime));
            yield return null;
        }
        SetScroll(x);
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

        float x = mouse.position.ReadValue().x / Screen.width;
        float dir = 0f;
        if (x > 1f - edgeSize) dir = -1f;   // mouse on the right, move content left
        else if (x < edgeSize) dir = 1f;

        SetScroll(content.anchoredPosition.x + dir * scrollSpeed * Time.deltaTime);
    }

    void SetScroll(float x)
    {
        content.anchoredPosition = new Vector2(ClampScroll(x), content.anchoredPosition.y);
    }

    // Keeps the pages from scrolling too far past the first/last page.
    float ClampScroll(float x)
    {
        Bounds pages = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, content);
        Rect view = viewport.rect;
        float pad = edgePadding * view.width;
        float current = content.anchoredPosition.x;

        float min = current + (view.xMax - pad) - pages.max.x;
        float max = current + (view.xMin + pad) - pages.min.x;
        if (max < min) max = min;

        return Mathf.Clamp(x, min, max);
    }

    static bool JournalPressed()
    {
        Keyboard kb = Keyboard.current;
        return kb != null && kb.sKey.wasPressedThisFrame;
    }
}