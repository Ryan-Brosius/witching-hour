using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Tooltip("Set this texture's Texture Type to Cursor in its import settings.")]
    public Texture2D hoverCursor;
    [Tooltip("The click point, in pixels from the top-left of the texture.")]
    public Vector2 hotspot;

    void Awake() => Instance = this;

    public void SetHover(bool on)
    {
        Cursor.SetCursor(on ? hoverCursor : null, hotspot, CursorMode.Auto);
    }
}
