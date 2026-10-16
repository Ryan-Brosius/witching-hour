using UnityEngine;

public class KnockDoor : Interactable
{
    public KnockPuzzle puzzle;
    public AudioSource knockSound;

    public override void Interact(PlayerInteractor player)
    {
        if (knockSound != null) knockSound.Play();
        if (puzzle != null) puzzle.Knock(this);
    }
}
