using UnityEngine;

[DefaultExecutionOrder(100)]
[ExecuteAlways]
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2.5f, -10f);
    [Tooltip("How much the camera looks down at the player.")]
    public float pitch = 8f;
    [Tooltip("Higher = lazier, floatier follow.")]
    public float smoothTime = 0.25f;

    [Tooltip("Copy the target's rotation instead of using pitch (used when zooming into a clock).")]
    public bool matchTargetRotation;
    [Tooltip("How quickly the camera turns to its new rotation.")]
    public float turnSpeed = 8f;

    [Header("Shake")]
    [Tooltip("Used by Shake() with no arguments, e.g. from a UnityEvent.")]
    public float shakeDuration = 0.3f;
    public float shakeStrength = 0.2f;

    Vector3 velocity;
    Vector3 followPos;
    float shakeTime;
    float shakeLength;
    float shakeAmount;

    void Start()
    {
        followPos = target.position + offset;
        transform.position = followPos;
        transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothTime);
        Quaternion goal = matchTargetRotation ? target.rotation : Quaternion.Euler(pitch, 0f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, goal, turnSpeed * Time.deltaTime);
    }

    public void Shake() => Shake(shakeDuration, shakeStrength);

    public void Shake(float duration, float strength)
    {
        shakeLength = duration;
        shakeTime = duration;
        shakeAmount = strength;
    }

    Vector3 ShakeOffset()
    {
        if (shakeTime <= 0f) return Vector3.zero;
        shakeTime -= Time.deltaTime;

        // Fades out over the duration.
        float fade = Mathf.Clamp01(shakeTime / shakeLength);
        return (Vector3)Random.insideUnitCircle * shakeAmount * fade;
    }
}