using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumberWheel : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler
{
    public TMP_Text label;
    [Range(0, 9)] public int value;
    [Tooltip("Stuck wheels can't be turned, forcing this number.")]
    public bool stuck;
    [Tooltip("Pixels of drag per number.")]
    public float dragStep = 40f;
    public AudioSource clickSound;

    CombinationLock lockPuzzle;
    float dragAmount;

    void Awake()
    {
        lockPuzzle = GetComponentInParent<CombinationLock>();
        label.text = value.ToString();
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (e.dragging) return;
        Shift(1);
    }

    public void OnBeginDrag(PointerEventData e) => dragAmount = 0f;

    public void OnDrag(PointerEventData e)
    {
        dragAmount += e.delta.y;
        while (dragAmount >= dragStep) { Shift(1); dragAmount -= dragStep; }
        while (dragAmount <= -dragStep) { Shift(-1); dragAmount += dragStep; }
    }

    void Shift(int amount)
    {
        if (stuck || lockPuzzle.Solved) return;

        value = (value + amount + 10) % 10;
        label.text = value.ToString();
        if (clickSound != null) clickSound.Play();
        lockPuzzle.Check();
    }
}
