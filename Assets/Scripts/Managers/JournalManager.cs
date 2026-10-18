using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Notepad-style journal. Pages are stacked direct children of Content, first child = first page.
// Each page needs its Pivot Y set to 1 (top) so it flips from the binding.
// Give each page a full-size child named "Back" (last child, so it covers the clues). It shows while the page is flipped over.
// To add a page, duplicate one and drag it to where you want it in the order.
public class JournalManager : MonoBehaviour
{
    public PlayerInteractor interactor;

    [Header("Layout")]
    public RectTransform panel;
    [Tooltip("Holds the pages. Each direct child is one page, top child = first page.")]
    public RectTransform content;

    [Header("Open / Close")]
    [Tooltip("How far below its normal spot the panel sits while hidden.")]
    public float slideDistance = 1200f;
    public float slideTime = 0.35f;

    [Header("Page Flip")]
    [Tooltip("Part of the screen height at the top/bottom that flips pages (0.1 = 10%).")]
    public float edgeSize = 0.1f;
    public float flipTime = 0.25f;
    [Tooltip("Wait after a flip before the next one can start.")]
    public float flipDelay = 0.15f;

    [Header("New Clues")]
    public float fadeTime = 0.6f;
    public float pauseAfterClue = 0.3f;

    readonly List<GameObject> queue = new List<GameObject>();
    readonly List<RectTransform> pages = new List<RectTransform>();
    int current;
    Vector2 shownPos;
    bool canFlip;
    bool flipping;

    void Awake()
    {
        shownPos = panel.anchoredPosition;
        panel.anchoredPosition = shownPos + Vector2.down * slideDistance;
        panel.gameObject.SetActive(false);

        // Clues from older runs :)
        foreach (Transform t in content.GetComponentsInChildren<Transform>(true))
            if (PlayerPrefs.GetInt(SaveKey(t.gameObject)) == 1) t.gameObject.SetActive(true);

        foreach (Transform child in content) pages.Add((RectTransform)child);

        // Reverse the draw order so the first page sits on top of the stack.
        foreach (RectTransform page in pages) page.SetAsFirstSibling();

        // Pages before the current one are flipped away.
        for (int i = 0; i < pages.Count; i++)
        {
            SetFlip(pages[i], 1f);
            pages[i].gameObject.SetActive(i >= current);
        }
    }

    void Update()
    {
        if (!interactor.Busy && JournalPressed())
            interactor.RunSequence(Journal());

        if (canFlip && !flipping) KeyFlip();

        //if (canFlip && !flipping) EdgeFlip();
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

        canFlip = true;
        do yield return null;
        while (!JournalPressed());
        canFlip = false;
        while (flipping) yield return null; // let a flip in progress finish

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

        yield return FlipTo(PageIndexOf(clue));

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            group.alpha = t / fadeTime;
            yield return null;
        }
        group.alpha = 1f;

        yield return new WaitForSeconds(pauseAfterClue);
    }

    // Walks up from the clue to the page it lives on (the direct child of Content).
    int PageIndexOf(GameObject clue)
    {
        Transform t = clue.transform;
        while (t.parent != null && t.parent != content) t = t.parent;
        return pages.IndexOf((RectTransform)t);
    }

    // Mouse at the bottom flips the front page up (next page), mouse at the top flips one back down.
    void EdgeFlip()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        float y = mouse.position.ReadValue().y / Screen.height;
        if (y < edgeSize && current < pages.Count - 1) StartCoroutine(Flip(1));
        else if (y > 1f - edgeSize && current > 0) StartCoroutine(Flip(-1));
    }

    // Flips one page at a time until the given page is in front.
    IEnumerator FlipTo(int index)
    {
        if (index < 0) yield break;
        while (current != index)
            yield return Flip(index > current ? 1 : -1);
    }

    IEnumerator Flip(int dir)
    {
        flipping = true;

        if (dir > 0)
        {
            // Front page folds up over the binding, showing its back, revealing the next one.
            RectTransform page = pages[current];
            current++;
            yield return FlipScale(page, 1f, -1f);
            page.gameObject.SetActive(false);
            SetFlip(page, 1f);
        }
        else
        {
            // Previous page folds back down from over the binding onto the front.
            current--;
            RectTransform page = pages[current];
            page.gameObject.SetActive(true);
            yield return FlipScale(page, -1f, 1f);
        }

        yield return new WaitForSeconds(flipDelay);
        flipping = false;
    }

    // Scaling Y through 0 around the top pivot reads as the page rotating over the binding.
    IEnumerator FlipScale(RectTransform page, float from, float to)
    {
        for (float t = 0f; t < flipTime; t += Time.deltaTime)
        {
            SetFlip(page, Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / flipTime)));
            yield return null;
        }
        SetFlip(page, to);
    }

    // Negative Y = page is past the binding, so show its back instead of the front.
    void SetFlip(RectTransform page, float scaleY)
    {
        page.localScale = new Vector3(1f, scaleY, 1f);
        Transform back = page.Find("Back");
        if (back != null) back.gameObject.SetActive(scaleY < 0f);
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

    static bool JournalPressed()
    {
        Keyboard kb = Keyboard.current;
        return kb != null && kb.sKey.wasPressedThisFrame;
    }

    void KeyFlip()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if ((kb.dKey.isPressed || kb.rightArrowKey.isPressed) && current < pages.Count - 1) StartCoroutine(Flip(1));
        else if ((kb.aKey.isPressed || kb.leftArrowKey.isPressed) && current > 0) StartCoroutine(Flip(-1));
    }
}