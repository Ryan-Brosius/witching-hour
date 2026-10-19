using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightsOut : MonoBehaviour
{
    public float fadeDuration = 3f;
    public Image lossScreen;

    private Light[] lights;

    public void StartFadeOut()
    {
        lights = FindObjectsByType<Light>(
            FindObjectsSortMode.None
        );

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;

        float[] startIntensities = new float[lights.Length];

        for (int i = 0; i < lights.Length; i++)
        {
            startIntensities[i] = lights[i].intensity;
        }

        if (lossScreen != null)
        {
            lossScreen.gameObject.SetActive(true);

            Color color = lossScreen.color;
            color.a = 0f;
            lossScreen.color = color;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i] != null)
                {
                    lights[i].intensity =
                        Mathf.Lerp(startIntensities[i], 0f, t);
                }
            }

            if (lossScreen != null)
            {
                Color color = lossScreen.color;
                color.a = t;
                lossScreen.color = color;
            }

            yield return null;
        }

        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.intensity = 0f;
                light.enabled = false;
            }
        }

        if (lossScreen != null)
        {
            Color color = lossScreen.color;
            color.a = 1f;
            lossScreen.color = color;
        }
    }
}
