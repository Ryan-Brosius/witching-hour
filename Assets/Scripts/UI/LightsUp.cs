using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightsUp : MonoBehaviour
{
    [Header("Directional Light")]
    public Light directionalLight;

    public float targetIntensity = 10f;
    public float startTemperature = 10000f;
    public float targetTemperature = 4500f;

    [Header("Lighting Transition")]
    public float lightingDuration = 4f;

    [Header("UI Panel")]
    public Image panelImage;
    public float panelFadeDuration = 2f;

    [Header("Ending Text")]
    public CanvasGroup endingText;
    public float textDelay = 1.5f;
    public float textFadeDuration = 2f;

    private float startingIntensity;
    private float startingTemperature;

    public void StartTransition()
    {
        if (directionalLight == null)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        startingIntensity = directionalLight.intensity;
        startingTemperature = startTemperature;

        directionalLight.useColorTemperature = true;
        directionalLight.colorTemperature = startTemperature;

        if (panelImage != null)
        {
            panelImage.gameObject.SetActive(true);

            Color color = panelImage.color;
            color.a = 0f;
            panelImage.color = color;
        }

        if (endingText != null)
        {
            endingText.alpha = 0f;
            endingText.gameObject.SetActive(false);
        }

        float elapsed = 0f;

        while (elapsed < lightingDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lightingDuration);

            directionalLight.intensity = Mathf.Lerp(startingIntensity, targetIntensity, t);

            directionalLight.colorTemperature = Mathf.Lerp(startingTemperature, targetTemperature, t);

            yield return null;
        }

        directionalLight.intensity = targetIntensity;
        directionalLight.colorTemperature = targetTemperature;

        if (panelImage != null)
        {
            float panelElapsed = 0f;

            while (panelElapsed < panelFadeDuration)
            {
                panelElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(panelElapsed / panelFadeDuration);

                Color color = panelImage.color;
                color.a = t;
                panelImage.color = color;

                yield return null;
            }

            Color finalColor = panelImage.color;
            finalColor.a = 1f;
            panelImage.color = finalColor;
        }

        yield return new WaitForSeconds(textDelay);

        if (endingText != null)
        {
            endingText.gameObject.SetActive(true);
            endingText.alpha = 0f;

            float textElapsed = 0f;

            while (textElapsed < textFadeDuration)
            {
                textElapsed += Time.deltaTime;

                endingText.alpha = Mathf.Clamp01(textElapsed / textFadeDuration);

                yield return null;
            }

            endingText.alpha = 1f;
            endingText.interactable = true;
            endingText.blocksRaycasts = true;
        }
    }
}
