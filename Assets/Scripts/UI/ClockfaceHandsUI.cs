using System.Collections.Generic;
using UnityEngine;

public class ClockfaceHandsUI : MonoBehaviour
{
    [System.Serializable]
    public struct ClockHands
    {
        public Transform hourHand;
        public Transform minuteHand;
    }

    [Range(0, 23)] public int hour = 2;
    [Range(0f, 60f)] public float minute = 50f;
    public float tweenTime = 0.5f;

    public List<ClockHands> clocks = new List<ClockHands>();

    float shown;
    float tweenFrom;
    float tweenStart = -1f;

    float Target => hour * 60f + minute;

    void Awake()
    {
        shown = Target;
    }

    void Update()
    {
        if (tweenStart >= 0f)
        {
            float t = Mathf.Clamp01((Time.time - tweenStart) / tweenTime);
            shown = Mathf.LerpUnclamped(tweenFrom, Target, EaseOutBack(t)); // small overshoot, like a real tick
            if (t >= 1f) tweenStart = -1f;
        }
        UpdateHands();
    }

    void OnValidate()
    {
        shown = Target;
        UpdateHands();
    }

    public void SetTime(int newHour, float newMinute)
    {
        hour = newHour;
        minute = newMinute;
        tweenFrom = shown;
        tweenStart = Time.time;
    }

    public ClockHands Detach(Transform hand)
    {
        int i = clocks.FindIndex(c => c.hourHand == hand || c.minuteHand == hand);
        if (i < 0) return default;

        ClockHands clock = clocks[i];
        clocks.RemoveAt(i);
        return clock;
    }

    void UpdateHands()
    {
        float minuteAngle = (shown % 60f) * 6f;
        float hourAngle = (shown / 60f % 12f) * 30f;

        foreach (ClockHands clock in clocks)
        {
            if (clock.minuteHand != null) clock.minuteHand.localEulerAngles = new Vector3(0f, 0f, -hourAngle);
            if (clock.hourHand != null) clock.hourHand.localEulerAngles = new Vector3(0f, 0f, -minuteAngle);
        }
    }

    static float EaseOutBack(float t)
    {
        const float s = 1.70158f;
        t -= 1f;
        return 1f + (s + 1f) * t * t * t + s * t * t;
    }
}