using UnityEngine;

// Slowly turns an object around Y. Used for the clock tower on the start screen.
public class SlowSpin : MonoBehaviour
{
    [Tooltip("Degrees per second.")]
    public float speed = 6f;

    void Update()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f, Space.World);
    }
}
