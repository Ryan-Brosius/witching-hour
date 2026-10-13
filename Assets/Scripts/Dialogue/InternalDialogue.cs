using System.Collections;
using TMPro;
using UnityEngine;

// The player's thoughts, shown on the UI. Types out, waits, fades. Doesn't block anything.
public class InternalDialogue : MonoBehaviour
{
    public TMP_Text text;
    public CanvasGroup group;
    public float charsPerSecond = 45f;
    [Tooltip("How long the line stays after it's fully shown.")]
    public float stayTime = 2.5f;
    public float fadeTime = 0.4f;

    Coroutine routine;

    void Awake()
    {
        group.alpha = 0f;
    }

    public void Say(string line)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Play(line));
    }

    IEnumerator Play(string line)
    {
        text.text = line;
        text.ForceMeshUpdate();
        int total = text.textInfo.characterCount;
        text.maxVisibleCharacters = 0;
        group.alpha = 1f;

        for (float t = 0f; text.maxVisibleCharacters < total; t += Time.deltaTime)
        {
            text.maxVisibleCharacters = Mathf.FloorToInt(t * charsPerSecond);
            yield return null;
        }

        yield return new WaitForSeconds(stayTime);

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            group.alpha = 1f - t / fadeTime;
            yield return null;
        }
        group.alpha = 0f;
    }
}
