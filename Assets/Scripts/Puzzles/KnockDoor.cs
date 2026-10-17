using System.Collections;
using UnityEngine;

public class KnockDoor : Interactable
{
    public KnockPuzzle puzzle;
    public AudioSource knockSound;

    [Header("Optional Reply")]
    public DialogueSpeaker speaker;
    [TextArea] public string[] lines;
    public float replyDelay = 0.5f;

    public override void Interact(PlayerInteractor player)
    {
        if (knockSound != null) knockSound.Play();
        if (puzzle != null) puzzle.Knock(this);

        if (speaker != null && lines.Length > 0)
            player.RunSequence(Reply());
    }

    IEnumerator Reply()
    {
        yield return new WaitForSeconds(replyDelay);
        yield return speaker.Say(lines);
    }
}
