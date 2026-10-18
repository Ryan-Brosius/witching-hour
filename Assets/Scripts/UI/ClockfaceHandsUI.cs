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
    [Tooltip("Can be fractional (e.g. 50.5) so the hands move smoothly.")]
    [Range(0f, 60f)] public float minute = 50f;

    public List<ClockHands> clocks = new List<ClockHands>();

    void Update()
    {
        UpdateHands();
    }

    void OnValidate()
    {
        UpdateHands();
    }

    public void SetTime(int newHour, float newMinute)
    {
        hour = newHour;
        minute = newMinute;
        UpdateHands();
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
        float minuteAngle = minute * 6f;
        float hourAngle = (hour % 12) * 30f + minute * 0.5f;

        foreach (ClockHands clock in clocks)
        {
            if (clock.minuteHand != null) clock.minuteHand.localEulerAngles = new Vector3(0f, 0f, -hourAngle);
            if (clock.hourHand != null) clock.hourHand.localEulerAngles = new Vector3(0f, 0f, -minuteAngle);
        }
    }
}