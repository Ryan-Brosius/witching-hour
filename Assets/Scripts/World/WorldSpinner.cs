using UnityEngine;

public class WorldSpinner : MonoBehaviour
{
    public WorldSpinData data;

    [Header("Gizmos")]
    public Color ringColor = new Color(1f, 0.8f, 0.2f);
    [Tooltip("Width of the guide lanes drawn either side of the walk path (houses inside, props outside).")]
    public float laneWidth = 3f;
    public int tickCount = 12;

    float moveInput;

    // -1 = left, 1 = right, 0 = no input.
    public float MoveInput => moveInput;
    public float Speed { get; private set; }
    public float Speed01 => data.maxSpeed > 0f ? Speed / data.maxSpeed : 0f;
    public Vector3 PlayerSpot => transform.position + Vector3.back * data.walkRadius;

    // Called by the input script.
    public void SetMoveInput(float input)
    {
        moveInput = Mathf.Clamp(input, -1f, 1f);
    }

    void Update()
    {
        // Accelerate when pushing the way we're already going, otherwise brake.
        float targetSpeed = moveInput * data.maxSpeed;
        bool speedingUp = moveInput != 0f && (Speed == 0f || Mathf.Sign(moveInput) == Mathf.Sign(Speed));
        float rate = speedingUp ? data.acceleration : data.deceleration;
        Speed = Mathf.MoveTowards(Speed, targetSpeed, rate * Time.deltaTime);

        // Turn walking speed into degrees, so the feel stays the same if the radius changes.
        float degrees = Speed / data.walkRadius * Mathf.Rad2Deg * Time.deltaTime;
        if (data.invertDirection) degrees = -degrees;
        transform.Rotate(0f, degrees, 0f, Space.World);

        // Push the moved colliders to physics now, so the player's ground check this frame is accurate.
        Physics.SyncTransforms();
    }

    void OnDrawGizmos()
    {
        if (data == null) return;

        Vector3 center = transform.position;
        float r = data.walkRadius;

        // Walk path
        Gizmos.color = ringColor;
        DrawCircle(center, r);

        Gizmos.color = new Color(ringColor.r, ringColor.g, ringColor.b, 0.3f);
        DrawCircle(center, r - laneWidth);
        DrawCircle(center, r + laneWidth);

        // Ticks that turn with the world, so you can see it spin in play mode
        Gizmos.color = ringColor;
        for (int i = 0; i < tickCount; i++)
        {
            Vector3 dir = transform.rotation * Quaternion.Euler(0f, i * 360f / tickCount, 0f) * Vector3.forward;
            Gizmos.DrawLine(center + dir * (r - 0.5f), center + dir * (r + 0.5f));
        }

        // Center axis (where the clock tower goes)
        Gizmos.color = Color.white;
        Gizmos.DrawLine(center, center + Vector3.up * 15f);

        // Where the player stands, and the direction the camera looks from
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(PlayerSpot + Vector3.up * 0.5f, 0.5f);
        Gizmos.DrawLine(PlayerSpot, PlayerSpot + Vector3.back * 4f);
    }

    void DrawCircle(Vector3 center, float radius, int segments = 64)
    {
        Vector3 prev = center + Vector3.forward * radius;
        for (int i = 1; i <= segments; i++)
        {
            float a = i * Mathf.PI * 2f / segments;
            Vector3 next = center + new Vector3(Mathf.Sin(a), 0f, Mathf.Cos(a)) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}
