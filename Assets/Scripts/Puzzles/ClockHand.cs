using UnityEngine;
using UnityEngine.EventSystems;

public class ClockHand : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float angle;
    [Tooltip("Snap to steps of this many degrees. 6 = one minute, 0 = free.")]
    public float snapDegrees = 6f;

    ClockPuzzle clock;

    void Awake()
    {
        clock = GetComponentInParent<ClockPuzzle>();
        SetAngle(angle);
    }

    public void OnDrag(PointerEventData e)
    {
        if (clock.Solved) return;

        // Point the hand at the mouse.
        Vector2 center = RectTransformUtility.WorldToScreenPoint(e.pressEventCamera, transform.position);
        Vector2 dir = e.position - center;
        float a = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg - 90f;
        if (snapDegrees > 0f) a = Mathf.Round(a / snapDegrees) * snapDegrees;

        SetAngle(a);
    }

    public void OnEndDrag(PointerEventData e)
    {
        clock.Check();
    }

    void SetAngle(float a)
    {
        angle = Mathf.Repeat(a, 360f);
        transform.localEulerAngles = new Vector3(0f, 0f, -angle);
    }
}
