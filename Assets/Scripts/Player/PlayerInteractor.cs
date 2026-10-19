using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Goes on the Player. Highlights the closest interactable and sends it the W / Up presses.
public class PlayerInteractor : MonoBehaviour
{
    [Tooltip("Ignore interactables further than this in front of / behind the player (e.g. the far side of the ring).")]
    public float maxDepth = 12f;
    [Tooltip("A material using the Custom/InteractOutline shader.")]
    public Material outlineMaterial;
    public float outlineWidth = 0.05f;
    public InternalDialogue internalDialogue;

    // True while a sequence (dialogue, knock animation, etc.) runs. Movement and interacting are blocked.
    public bool Busy { get; set; }
    public Interactable Current { get; private set; }

    Interactable held;
    float pressTime;

    void Update()
    {
        // Let the held object know when W is let go (for tap vs hold).
        if (held != null && InteractUp())
        {
            held.InteractReleased(this, Time.time - pressTime);
            held = null;
        }

        if (Busy) return;

        SetCurrent(FindClosest());
        if (Current == null || !InteractDown()) return;

        if (!Current.canInteract && !string.IsNullOrEmpty(Current.lockedHint))
        {
            internalDialogue.Say(Current.lockedHint);
            return;
        }

        held = Current;
        pressTime = Time.time;
        Current.onInteract.Invoke();
        Current.Interact(this);
    }

    Interactable FindClosest()
    {
        Interactable best = null;
        float bestDist = float.MaxValue;
        Vector3 p = transform.position;

        foreach (Interactable i in Interactable.All)
        {
            if (!i.Selectable) continue;

            Vector3 o = i.transform.position;
            if (Mathf.Abs(o.z - p.z) > maxDepth) continue;

            // Sidescroller: only left/right distance matters.
            float dist = Mathf.Abs(o.x - p.x);
            if (dist <= i.range && dist < bestDist)
            {
                best = i;
                bestDist = dist;
            }
        }
        return best;
    }

    void SetCurrent(Interactable next)
    {
        if (next == Current) return;
        if (Current != null) Current.SetHighlighted(false, outlineMaterial, outlineWidth);
        Current = next;
        if (Current != null) Current.SetHighlighted(true, outlineMaterial, outlineWidth);
    }

    // Locks the player until the routine finishes. Usage: player.RunSequence(MyRoutine());
    public void RunSequence(IEnumerator routine)
    {
        if (!Busy) StartCoroutine(Sequence(routine));
    }

    IEnumerator Sequence(IEnumerator routine)
    {
        Busy = true;
        SetCurrent(null);
        yield return routine;
        Busy = false;
    }

    // ---- Input helpers, usable from any script ----

    public static bool InteractDown()
    {
        Keyboard kb = Keyboard.current;
        Gamepad pad = Gamepad.current;
        return (kb != null && (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame))
            || (pad != null && pad.buttonSouth.wasPressedThisFrame);
    }

    public static bool InteractUp()
    {
        Keyboard kb = Keyboard.current;
        Gamepad pad = Gamepad.current;
        return (kb != null && (kb.wKey.wasReleasedThisFrame || kb.upArrowKey.wasReleasedThisFrame))
            || (pad != null && pad.buttonSouth.wasReleasedThisFrame);
    }

    // In a sequence: yield return PlayerInteractor.WaitForInteract();
    // Skips the current frame so the press that started the sequence doesn't count.
    public static IEnumerator WaitForInteract()
    {
        do yield return null;
        while (!InteractDown());
    }
}
