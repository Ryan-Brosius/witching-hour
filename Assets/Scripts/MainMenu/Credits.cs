using UnityEngine;
using UnityEngine.InputSystem;

public class Credits : MonoBehaviour
{
    public RectTransform content;
    public float scrollSpeed = 60f;
    public float fastMultiplier = 4f;

    void OnEnable()
    {
        float viewHeight = ((RectTransform)content.parent).rect.height;
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, -viewHeight);
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null && kb.escapeKey.wasPressedThisFrame)
        {
            gameObject.SetActive(false);
            return;
        }

        bool fast = kb != null && (kb.wKey.isPressed || kb.upArrowKey.isPressed || kb.spaceKey.isPressed);
        float speed = scrollSpeed * (fast ? fastMultiplier : 1f);

        Vector2 pos = content.anchoredPosition;
        pos.y += speed * Time.deltaTime;
        content.anchoredPosition = pos;

        if (pos.y >= content.rect.height) gameObject.SetActive(false);
    }
}
