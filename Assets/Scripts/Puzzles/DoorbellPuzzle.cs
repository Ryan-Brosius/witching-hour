using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorbellPuzzle : MonoBehaviour
{
    public Doorbell bell;
    [Tooltip(". = short ring, - = long ring")]
    public string solution = "..--- ..... -----";
    public UnityEvent onSolved;

    public bool Solved { get; private set; }

    readonly List<char> symbols = new List<char>();
    int index;

    void Awake()
    {
        string[] groups = solution.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int g = 0; g < groups.Length; g++)
        {
            foreach (char c in groups[g])
            {
                symbols.Add(c);
            }
        }
    }

    // Called by Doorbell. Any mistake starts over
    public void Ring(Doorbell bell, char symbol)
    {
        if (Solved) return;

        if (Matches(index, bell, symbol)) index++;
        else index = Matches(0, bell, symbol) ? 1 : 0;

        if (index < symbols.Count) return;
        Solved = true;
        onSolved.Invoke();
    }

    bool Matches(int i, Doorbell bell, char symbol) => symbols[i] == symbol;
}
