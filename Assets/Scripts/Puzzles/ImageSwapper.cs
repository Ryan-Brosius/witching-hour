using UnityEngine;
using UnityEngine.UI;

public class ImageSwapper : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite openSprite;
    public Sprite closedSprite;

    [Header("References")]
    public Image imageRenderer;
    public Interactable gateInteractable;
    public bool alwaysOpen = false;

    public void SwapSprite()
    {
        if (imageRenderer == null) return;

        if (alwaysOpen)
        {
            imageRenderer.sprite = openSprite;
            return;
        }

        imageRenderer.sprite = gateInteractable.canInteract ? openSprite : closedSprite;
    }
}
