using UnityEngine;

public class SpriteSwapperGate : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite openSprite;
    public Sprite closedSprite;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Interactable gateInteractable;
    public bool alwaysOpen = false;

    public void SwapSprite()
    {
        if (spriteRenderer == null) return;

        if (alwaysOpen)
        {
            spriteRenderer.sprite = openSprite;
            return;
        }

        spriteRenderer.sprite = gateInteractable.canInteract ? openSprite : closedSprite;
    }
}
