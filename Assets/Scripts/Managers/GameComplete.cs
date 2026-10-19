using UnityEngine;
using UnityEngine.Events;

// Fires onComplete once every clock puzzle is solved and the player is free again.
public class GameComplete : MonoBehaviour
{
    public ClockPuzzle[] puzzles;
    public PlayerInteractor interactor;
    public TimeManager timeManager;
    public UnityEvent onComplete;
    [Tooltip("Sends how long the run took, in in-game seconds.")]
    public UnityEvent<float> onCompleteTime;

    bool done;

    void Update()
    {
        if (done || interactor.Busy) return;

        foreach (ClockPuzzle puzzle in puzzles)
            if (!puzzle.Solved) return;

        done = true;
        timeManager.enabled = false; // stop the clock so time can't run out after winning
        onComplete.Invoke();
        onCompleteTime.Invoke(timeManager.GameSeconds);
    }
}