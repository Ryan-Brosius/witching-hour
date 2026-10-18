using System.Collections;
using UnityEngine;

public class SceneFadeIn : MonoBehaviour
{
    public CanvasGroup group;
    public float fadeTime = 1f;
    [Tooltip("Wait this long on black before fading.")]
    public float delay = 0.2f;

    void Awake()
    {
        group.alpha = 1f;
        group.blocksRaycasts = true;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            group.alpha = 1f - t / fadeTime;
            yield return null;
        }

        group.alpha = 0f;
        group.blocksRaycasts = false;
    }
}
