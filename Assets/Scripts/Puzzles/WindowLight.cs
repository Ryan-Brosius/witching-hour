using UnityEngine;
using UnityEngine.EventSystems;

public class WindowLight : MonoBehaviour, IPointerClickHandler
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
}
