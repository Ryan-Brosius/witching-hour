using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

// Goes on the Metal Gate. Press W to zoom in on a clock face on the tower and drag its hands.
// Solving it, or pressing W again, zooms back out.
public class ClockPuzzle : Interactable
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

    [Header("Zoom")]
    [Tooltip("Empty in front of the clock face. The camera sits here and copies its rotation, so point its forward (blue) arrow at the clock.")]
    public Transform zoomPoint;
    [Tooltip("The script ticking the tower's clocks. This clock is taken out of it while you hold the hands, and for good once solved.")]
    public ClockfaceHandsUI clockface;
    public float solvedExitDelay = 1f;

    public bool Solved { get; private set; }
    public bool Engaged { get; private set; }

    CameraFollow cam;

    void Awake()
    {
        cam = Camera.main.GetComponent<CameraFollow>();
        hourHand.clock = this;
        minuteHand.clock = this;
    }

    public override void Interact(PlayerInteractor player)
    {
        player.RunSequence(Inspect());
    }

    IEnumerator Inspect()
    {
        Transform oldTarget = cam.target;
        Vector3 oldOffset = cam.offset;
        bool oldCursor = Cursor.visible;

        cam.target = zoomPoint;
        cam.offset = Vector3.zero;
        cam.matchTargetRotation = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ClockfaceHandsUI.ClockHands ticking = clockface.Detach(minuteHand.transform);
        Engaged = true;

        bool wasSolved = Solved;
        do yield return null;
        while (!PlayerInteractor.InteractDown() && (wasSolved || !Solved));

        Engaged = false;
        if (!wasSolved && Solved) yield return new WaitForSeconds(solvedExitDelay);

        // Left without solving: let the clock tick again.
        if (!Solved && ticking.hourHand != null) clockface.clocks.Add(ticking);

        cam.target = oldTarget;
        cam.offset = oldOffset;
        cam.matchTargetRotation = false;
        Cursor.visible = oldCursor;
    }

    public void Check()
    {
        if (Solved || !Close(hourHand.Angle, hourTarget) || !Close(minuteHand.Angle, minuteTarget)) return;
        Solved = true;
        onSolved.Invoke();
    }

    bool Close(float a, float b) => Mathf.Abs(Mathf.DeltaAngle(a, b)) <= tolerance;

    #if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Vector3 center = hourHand != null ? hourHand.transform.position : transform.position;
        RectTransform rt = transform as RectTransform;

        DrawHand(center, 1.0f, hourTarget, hourHand, new Color(0.3f, 0.8f, 1f));
        DrawHand(center, 1.5f, minuteTarget, minuteHand, new Color(1f, 0.8f, 0.2f));
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
            Gizmos.color = Close(hand.Angle, target) ? Color.green : Color.red;
            Vector3 tip = center + Dir(hand.Angle) * length * 1.05f;
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
    #endif
}