using UnityEngine;
using UnityEngine.Events;

public class KnockPuzzle : MonoBehaviour
{
    [Tooltip("Doors in the order they must be knocked. Repeat a door to require multiple knocks.")]
    public KnockDoor[] sequence;
    public UnityEvent onSolved;

    public bool Solved { get; private set; }

    int index;

    public void Knock(KnockDoor door)
    {
        if (Solved) return;

        if (door == sequence[index]) index++;
        else index = door == sequence[0] ? 1 : 0;

        if (index < sequence.Length) return;
        Solved = true;
        onSolved.Invoke();
    }
}
