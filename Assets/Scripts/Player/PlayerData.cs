using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Ground")]
    [Tooltip("Set this to your Ground layer so the ray doesn't hit the player or props.")]
    public LayerMask groundLayers = 1;
    [Tooltip("How far above/below the player we look for ground. Bigger = handles taller steps.")]
    public float groundCheckHeight = 10f;
    [Tooltip("Smooths height changes over bumps. 0 = snap instantly.")]
    public float groundSmoothTime = 0.06f;

    [Header("Hop")]
    public float hopHeight = 0.15f;
    [Tooltip("Hops per unit walked, so the step rate matches how fast the ground moves.")]
    public float hopsPerUnit = 1.2f;

    [Header("Facing")]
    [Tooltip("Turn on if the sprite art is drawn facing left.")]
    public bool artFacesLeft;
    [Tooltip("Degrees per second for the paper flip. 900 = about 0.2s per turn around.")]
    public float turnSpeed = 900f;

    [Header("Lean")]
    [Tooltip("How far the player tips into the walk at full speed.")]
    public float leanAngle = 6f;
    public float leanSmoothTime = 0.12f;
}
