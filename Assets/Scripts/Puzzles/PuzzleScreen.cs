using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleScreen : MonoBehaviour
{
    public CanvasGroup group;
    [Tooltip("The part that scales in, usually the main panel.")]
    public RectTransform panel;
    public float openTime = 0.25f;
    public float closeTime = 0.15f;
    [Tooltip("How long the solved puzzle stays up before closing.")]
    public float solvedCloseDelay = 1f;

    bool open;

    public IEnumerator Show()
    {
        open = true;
        gameObject.SetActive(true);
        yield return Animate(true);

        while (open)
        {
            if (PlayerInteractor.InteractDown()) open = false;
            yield return null;
        }

        yield return Animate(false);
        gameObject.SetActive(false);
    }

    public void Close() => open = false;

    public void CloseAfterSolve() => StartCoroutine(CloseLater());

    IEnumerator CloseLater()
    {
        yield return new WaitForSeconds(solvedCloseDelay);
        open = false;
    }

    IEnumerator Animate(bool opening)
    {
        float time = opening ? openTime : closeTime;
        for (float t = 0f; t < time; t += Time.deltaTime)
        {
            SetLook(opening, t / time);
            yield return null;
        }
        SetLook(opening, 1f);
    }

    void SetLook(bool opening, float t)
    {
        group.alpha = opening ? t : 1f - t;
        panel.localScale = Vector3.one * (opening ? EaseOutBack(t) : 1f - t * t);
    }

    static float EaseOutBack(float t)
    {
        const float s = 1.70158f;
        t -= 1f;
        return 1f + (s + 1f) * t * t * t + s * t * t;
    }
}
