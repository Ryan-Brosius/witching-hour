using UnityEngine;

// Press W to open a UI screen (puzzle, book page, map). Movement is locked while it's open.
public class ScreenInteractable : Interactable
{
    public PuzzleScreen screen;

    public override void Interact(PlayerInteractor player)
    {
        player.RunSequence(screen.Show());
    }

    // Hook to a puzzle's onSolved so the next press opens something else (e.g. the book page).
    public void SetScreen(PuzzleScreen next)
    {
        screen = next;
    }
}
