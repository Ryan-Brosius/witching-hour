using UnityEngine;
using UnityEngine.Events;

public class GameComplete : MonoBehaviour
{
    public ClockPuzzle[] puzzles;
    public PlayerInteractor interactor;
    public UnityEvent onComplete;

    bool done;

    void Update()
    {
        if (done || interactor.Busy) return;

        foreach (ClockPuzzle puzzle in puzzles)
            if (!puzzle.Solved) return;

        done = true;
        onComplete.Invoke();
    }
}
