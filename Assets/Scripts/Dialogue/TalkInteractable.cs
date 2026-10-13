using UnityEngine;

// Press W next to a character to have them say their lines in the world.
public class TalkInteractable : Interactable
{
    public DialogueSpeaker speaker;
    [TextArea] public string[] lines;

    public override void Interact(PlayerInteractor player)
    {
        player.RunSequence(speaker.Say(lines));
    }
}
