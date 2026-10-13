using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerData data;
    public WorldSpinner world;
    [Tooltip("Child holding the model.")]
    public Transform visual;

    Vector3 visualStartPos;
    float groundVelocity;
    float hopPhase;
    float facingYaw;
    float lean;
    float leanVelocity;

    void Start()
    {
        Vector3 spot = world.PlayerSpot;
        transform.position = new Vector3(spot.x, transform.position.y, spot.z);

        visualStartPos = visual.localPosition;
        facingYaw = data.faceAngle;
    }

    void LateUpdate()
    {
        StickToGround();
        AnimateVisual();
    }

    void StickToGround()
    {
        Vector3 origin = transform.position + Vector3.up * data.groundCheckHeight;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, data.groundCheckHeight * 2f, data.groundLayers))
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.SmoothDamp(pos.y, hit.point.y, ref groundVelocity, data.groundSmoothTime);
            transform.position = pos;
        }
    }

    void AnimateVisual()
    {
        float speed01 = world.Speed01;          // -1..1
        float amount = Mathf.Abs(speed01);      // 0..1

        // Hop, scaled by speed so it settles back down when we stop.
        hopPhase = (hopPhase + Mathf.Abs(world.Speed) * data.hopsPerUnit * Time.deltaTime) % 1f;
        float hop = Mathf.Abs(Mathf.Sin(hopPhase * Mathf.PI)) * data.hopHeight * amount;
        visual.localPosition = visualStartPos + Vector3.up * hop;

        // Face the input direction right away (keeps the last facing when idle).
        if (world.MoveInput > 0f) facingYaw = Mathf.MoveTowardsAngle(facingYaw, data.faceAngle, data.turnSpeed * Time.deltaTime);
        if (world.MoveInput < 0f) facingYaw = Mathf.MoveTowardsAngle(facingYaw, -data.faceAngle, data.turnSpeed * Time.deltaTime);

        // Lean slightly into the walk.
        lean = Mathf.SmoothDamp(lean, -speed01 * data.leanAngle, ref leanVelocity, data.leanSmoothTime);

        visual.rotation = Quaternion.Euler(0f, 0f, lean) * Quaternion.Euler(0f, facingYaw, 0f);
    }
}
