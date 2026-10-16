using System.Collections;
using UnityEngine;

// Put on a house with a doorbell. Tap W for a short ring, hold for a long one.
public class Doorbell : Interactable
{
    public DoorbellPuzzle puzzle;
    public AudioSource ringSound;
    [Tooltip("Held at least this long counts as a long ring.")]
    public float holdThreshold = 0.5f;
    public float shortRing = 0.3f;
    public float longRing = 1f;

    public override void Interact(PlayerInteractor player)
    {
        StopAllCoroutines();
        ringSound.Play();
    }

    public override void InteractReleased(PlayerInteractor player, float heldTime)
    {
        bool isLong = heldTime >= holdThreshold;

        // Stop the chime so its total length matches the ring type.
        // Fix later so it sounds better :)
        StartCoroutine(StopRing((isLong ? longRing : shortRing) - heldTime));

        if (puzzle != null) puzzle.Ring(this, isLong ? '-' : '.');
    }

    IEnumerator StopRing(float delay)
    {
        yield return new WaitForSeconds(delay);
        ringSound.Stop();
    }
}
