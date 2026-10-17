using UnityEditor;
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

    void OnDrawGizmos()
    {
        Vector3 center = hourHand != null ? hourHand.transform.position : transform.position;
        RectTransform rt = transform as RectTransform;

        DrawHand(center, 200.0f, hourTarget, hourHand, new Color(0.3f, 0.8f, 1f));
        DrawHand(center, 350.0f, minuteTarget, minuteHand, new Color(1f, 0.8f, 0.2f));
    }

    void DrawHand(Vector3 center, float length, float target, ClockHand hand, Color color)
    {
        Gizmos.color = new Color(color.r, color.g, color.b, 0.4f);
        Gizmos.DrawLine(center, center + Dir(target - tolerance) * length);
        Gizmos.DrawLine(center, center + Dir(target + tolerance) * length);
        Vector3 prev = center + Dir(target - tolerance) * length;
        for (int i = 1; i <= 16; i++)
        {
            Vector3 next = center + Dir(Mathf.Lerp(target - tolerance, target + tolerance, i / 16f)) * length;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }

        Gizmos.color = color;
        Gizmos.DrawLine(center, center + Dir(target) * length);

        if (hand != null)
        {
            Gizmos.color = Close(hand.angle, target) ? Color.green : Color.red;
            Vector3 tip = center + Dir(hand.angle) * length * 1.05f;
            Gizmos.DrawLine(center, tip);
            Gizmos.DrawWireSphere(tip, length * 0.04f);
        }

        Handles.color = color;
    }

    Vector3 Dir(float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        return transform.rotation * new Vector3(Mathf.Sin(r), Mathf.Cos(r), 0f);
    }
}
