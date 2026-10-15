using UnityEngine;
using UnityEngine.Events;

public class LampPuzzle : MonoBehaviour
{
    [Tooltip("In order: #1 is east of the North Gate, going clockwise.")]
    public StreetLamp[] lamps;
    [Tooltip("1 = on, 0 = off, one per lamp in order. Spaces are ignored.")]
    public string solution = "1111 1010";
    public UnityEvent onSolved;

    public bool Solved { get; private set; }

    void Update()
    {
        if (Solved) return;

        string pattern = solution.Replace(" ", "");
        for (int i = 0; i < lamps.Length; i++)
            if (lamps[i].isOn != (pattern[i] == '1')) return;

        Solved = true;
        onSolved.Invoke();
    }

    public void UnlockLamps()
    {
        foreach (StreetLamp lamp in lamps) lamp.canInteract = true;
    }
}
