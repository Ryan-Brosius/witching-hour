using TMPro;
using UnityEngine;

public class CompassBar : MonoBehaviour
{
    public WorldSpinner world;
    public RectTransform bar;
    [Tooltip("In order: N, E, S, W.")]
    public GameObject[] letters;

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
            GameObject letterObj = letters[i];
            if (letterObj == null) continue;

            float delta = Mathf.DeltaAngle(heading, i * 90f);
            float t = Mathf.Abs(delta) / visibleAngle;

            bool isVisible = t <= 1f;
            letterObj.SetActive(isVisible);
            if (!isVisible) continue;

            RectTransform rt = letterObj.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(delta / visibleAngle * halfWidth, rt.anchoredPosition.y);
                rt.localScale = Vector3.one * Mathf.Lerp(centerScale, edgeScale, t);
            }
        }
    }
}
