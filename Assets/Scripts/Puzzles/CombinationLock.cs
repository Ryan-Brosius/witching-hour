using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CombinationLock : MonoBehaviour
{
    [Tooltip("Left to right.")]
    public NumberWheel[] wheels;
    public bool useEvaluation = true;
    public bool usePowers = false;
    [Tooltip("Symbols between the wheels, left to right. Use + - x / ^")]
    public string operators = "xxx";
    public float target = 250f;
    public UnityEvent onSolved;

    public bool Solved { get; private set; }

    // Called by the wheels whenever one turns.
    public void Check()
    {
        if (Solved || !Mathf.Approximately(Evaluate(), target)) return;
        Solved = true;
        onSolved.Invoke();
    }

    public float Evaluate()
    {
        if (!useEvaluation)
        {
            string combinedString = string.Concat(wheels.Select(w => w.value.ToString()));
            return float.Parse(combinedString);
        }
        float result = wheels[0].value;
        if (usePowers) result = Mathf.Pow(2, wheels[0].value);
        for (int i = 1; i < wheels.Length; i++)
        {
            float n = wheels[i].value;
            switch (operators[i - 1])
            {
                case '+': result += n; break;
                case '-': result -= n; break;
                case 'x': result *= n; break;
                case '/': result /= n; break;
                case '^': result = Mathf.Pow(result, n); break;
                case '@': result -= Mathf.Pow(2, n); break;
            }
        }
        return result;
    }
}
