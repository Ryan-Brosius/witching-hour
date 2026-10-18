using UnityEngine;
using UnityEngine.EventSystems;

public class ClockHand : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Tooltip("Snap to steps of this many degrees. 6 = one minute, 0 = free.")]
    public float snapDegrees = 6f;

    [HideInInspector] public ClockPuzzle clock; // set by ClockPuzzle

    public float Angle => Mathf.Repeat(-transform.localEulerAngles.z, 360f);

    public void OnDrag(PointerEventData e)
    {
        if (!clock.Engaged || clock.Solved) return;

        Vector2 center = RectTransformUtility.WorldToScreenPoint(e.pressEventCamera, transform.position);
        Vector2 dir = e.position - center;
        float a = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        if (snapDegrees > 0f) a = Mathf.Round(a / snapDegrees) * snapDegrees;

        transform.localEulerAngles = new Vector3(0f, 0f, -a);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (clock.Engaged) clock.Check();
    }
}