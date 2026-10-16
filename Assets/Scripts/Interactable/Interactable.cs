using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Put this on anything the player can press W on. Subclass it for custom behaviour,
// or just hook up onInteract in the inspector for simple stuff.
public class Interactable : MonoBehaviour
{
    // Every enabled interactable, so the player can find the closest one.
    public static readonly List<Interactable> All = new List<Interactable>();

    [Tooltip("How close (left/right) the player has to be.")]
    public float range = 1.5f;
    [Tooltip("Turn off until a puzzle or item unlocks this.")]
    public bool canInteract = true;
    [Tooltip("Internal dialogue shown when pressing W while locked. Leave empty to ignore this object while it's locked.")]
    [TextArea] public string lockedHint;
    [Tooltip("For simple things (play a sound, open a door) without writing a new script.")]
    public UnityEvent onInteract;

    InteractOutline outline;

    public bool Selectable => canInteract || !string.IsNullOrEmpty(lockedHint);

    void OnEnable() => All.Add(this);

    void OnDisable()
    {
        All.Remove(this);
        if (outline != null) outline.Show(false, null, 0f);
    }

    // Called the frame W / Up is pressed.
    public virtual void Interact(PlayerInteractor player) { }

    // Called when W / Up is let go, with how long it was held (doorbell tap vs hold).
    public virtual void InteractReleased(PlayerInteractor player, float heldTime) { }

    // For UnityEvents, e.g. a puzzle's onSolved unlocking a gate.
    public void Unlock() => canInteract = true;

    public void Lock() => canInteract &= false;

    public void SetHighlighted(bool on, Material material, float width)
    {
        if (outline == null) outline = gameObject.AddComponent<InteractOutline>();
        outline.Show(on, material, width);
    }
}