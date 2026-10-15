using UnityEngine;

public class StreetLamp : Interactable
{
    public bool isOn;
    [Tooltip("The Light, glow sprite, etc. Shown while on.")]
    public GameObject lightObject;
    public AudioSource toggleSound;

    void Start()
    {
        lightObject.SetActive(isOn);
    }

    public override void Interact(PlayerInteractor player)
    {
        isOn = !isOn;
        lightObject.SetActive(isOn);
        if (toggleSound != null) toggleSound.Play();
    }
}
