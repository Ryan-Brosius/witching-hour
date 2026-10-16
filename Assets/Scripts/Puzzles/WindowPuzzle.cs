using UnityEngine;
using UnityEngine.Events;

public class WindowPuzzle : MonoBehaviour
{
    [Tooltip("Every window light")]
    public WindowLight[] lights;
    [Tooltip("1 = on, 0 = off, one per light in order")]
    public string solution = "001111 110000";
    public UnityEvent onSolved;

    public bool Solved { get; private set; }

    void Update()
    {
        if (Solved) return;

        string pattern = solution.Replace(" ", "");
        for (int i = 0; i < lights.Length; i++)
            if (lights[i].isOn != (pattern[i] == '1')) return;

        Solved = true;
        onSolved.Invoke();
    }
}
