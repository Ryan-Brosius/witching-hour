using TMPro;
using UnityEngine;

public class CompassBar : MonoBehaviour
{
    public WorldSpinner world;
    public RectTransform bar;
    [Tooltip("In order: N, E, S, W.")]
    public TMP_Text[] letters;

    public float visibleAngle = 90f;
    [Tooltip("Letter size at the very edge of the bar.")]
    public float edgeScale = 0.5f;
    [Tooltip("Letter size center.")]
    public float centerScale = 1.3f;

    void LateUpdate()
    {
        float heading = world.transform.eulerAngles.y;
        float halfWidth = bar.rect.width * 0.5f;

        for (int i = 0; i < letters.Length; i++)
        {
            TMP_Text letter = letters[i];

            float delta = Mathf.DeltaAngle(heading, i * 90f);
            float t = Mathf.Abs(delta) / visibleAngle;

            letter.enabled = t <= 1f;
            if (!letter.enabled) continue;

            RectTransform rt = letter.rectTransform;
            rt.anchoredPosition = new Vector2(delta / visibleAngle * halfWidth, rt.anchoredPosition.y);
            rt.localScale = Vector3.one * Mathf.Lerp(centerScale, edgeScale, t);
        }
    }
}
