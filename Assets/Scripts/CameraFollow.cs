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

    Vector3 velocity;

    void Start()
    {
        transform.position = target.position + offset;
        transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothTime);
        transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
