using UnityEngine;

[CreateAssetMenu(fileName = "WorldSpinData", menuName = "Data/World Spin Data")]
public class WorldSpinData : ScriptableObject
{
    [Header("Ring")]
    [Tooltip("Distance from the world center (clock tower) to the path the player walks on.")]
    public float walkRadius = 20f;

    [Header("Movement")]
    [Tooltip("Top walking speed along the ring, in units per second.")]
    public float maxSpeed = 4f;
    [Tooltip("How quickly we reach top speed.")]
    public float acceleration = 12f;
    [Tooltip("How quickly we stop or turn around. Usually a bit higher than acceleration so stops feel snappy.")]
    public float deceleration = 18f;
    [Tooltip("Flip this if pressing right makes the world go the wrong way.")]
    public bool invertDirection;
}
