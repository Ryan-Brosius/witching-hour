using UnityEngine;
using UnityEngine.EventSystems;

public class WindowLight : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool isOn;
    [Tooltip("The glow sprite, Light, etc. Shown while on.")]
    public GameObject lightObject;
    public AudioSource toggleSound;

    WindowHouse house;

    void Awake()
    {
        house = GetComponentInParent<WindowHouse>();
    }

    void Start()
    {
        lightObject.SetActive(isOn);
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (!house.Engaged) return;

        isOn = !isOn;
        lightObject.SetActive(isOn);
        if (toggleSound != null) toggleSound.Play();
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (house.Engaged) CursorManager.Instance.SetHover(true);
    }

    public void OnPointerExit(PointerEventData e)
    {
        CursorManager.Instance.SetHover(false);
    }
}
