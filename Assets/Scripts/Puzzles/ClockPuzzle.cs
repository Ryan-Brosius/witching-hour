using UnityEngine;
using UnityEngine.Events;

public class ClockPuzzle : MonoBehaviour
{
    public ClockHand hourHand;
    public ClockHand minuteHand;
    [Tooltip("Degrees clockwise from 12. For a time: hour x 30 + minutes x 0.5 (2:50 = 85).")]
    public float hourTarget = 85f;
    [Tooltip("Degrees clockwise from 12. Minutes x 6 (50 = 300).")]
    public float minuteTarget = 300f;
    [Tooltip("How close (degrees) each hand needs to be.")]
    public float tolerance = 18f;
    public UnityEvent onSolved;

    public bool Solved { get; private set; }

    public void Check()
    {
        if (Solved || !Close(hourHand.angle, hourTarget) || !Close(minuteHand.angle, minuteTarget)) return;
        Solved = true;
        onSolved.Invoke();
        Debug.Log("Clock solved");
    }

    bool Close(float a, float b) => Mathf.Abs(Mathf.DeltaAngle(a, b)) <= tolerance;
}
