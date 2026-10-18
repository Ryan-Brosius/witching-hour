using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Counts down the 10 minutes before the Witching Hour.
// Every in-game minute it zooms the camera to the tower and ticks the clock forward.
public class TimeManager : MonoBehaviour
{
    public PlayerInteractor interactor;
    public ClockfaceHandsUI clockface;
    public CameraFollow cam;
    [Tooltip("Empty looking at the tower clock. The camera sits here and copies its rotation.")]
    public Transform zoomPoint;

    [Header("Timer")]
    [Tooltip("Real seconds per in-game minute.")]
    public float secondsPerMinute = 60f;
    public int totalMinutes = 10;
    [Tooltip("Keep counting while the player is busy")]
    public bool countWhileBusy;

    [Header("Tick Sequence")]
    [Tooltip("Time for the camera to pan over before the clock ticks, and to pan back after.")]
    public float panTime = 1f;
    [Tooltip("How long to stay on the clock after the hands finish moving.")]
    public float holdTime = 0.5f;

    [Tooltip("Runs when the last minute passes (the loss state).")]
    public UnityEvent onClockTick;
    [Tooltip("Runs when the last minute passes (the loss state).")]
    public UnityEvent onTimeUp;

    public int MinutesPassed { get; private set; }

    float timer;

    void Update()
    {
        if (MinutesPassed >= totalMinutes) return;
        if (interactor.Busy && !countWhileBusy) return;

        timer += Time.deltaTime;
        if (timer < secondsPerMinute) return;
        timer -= secondsPerMinute;
        MinutesPassed++;

        if (interactor.Busy)
        {
            AddMinute();
            CheckTimeUp();
        }
        else interactor.RunSequence(TickMinute());
    }

    IEnumerator TickMinute()
    {
        Transform oldTarget = cam.target;
        Vector3 oldOffset = cam.offset;

        cam.target = zoomPoint;
        cam.offset = Vector3.zero;
        cam.matchTargetRotation = true;
        yield return new WaitForSeconds(panTime);

        AddMinute();
        onClockTick.Invoke();
        cam.Shake(0.2f, 0.2f);
        yield return new WaitForSeconds(clockface.tweenTime + holdTime);

        cam.target = oldTarget;
        cam.offset = oldOffset;
        cam.matchTargetRotation = false;
        yield return new WaitForSeconds(panTime);

        CheckTimeUp();
    }

    void AddMinute()
    {
        int total = clockface.hour * 60 + Mathf.RoundToInt(clockface.minute) + 1;
        clockface.SetTime(total / 60, total % 60);
    }

    void CheckTimeUp()
    {
        if (MinutesPassed >= totalMinutes) onTimeUp.Invoke();
    }
}
