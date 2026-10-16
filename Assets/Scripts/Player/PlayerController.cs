using Unity.Collections;
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

    [Header("Sprite Lists")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] movingSprites;
    public Sprite[] stationarySprites;
    public float frameRate = 2f;
    bool isMoving = false;
    float timer;
    private int currentFrame;

    void Start()
    {
        Vector3 spot = world.PlayerSpot;
        transform.position = new Vector3(spot.x, transform.position.y, spot.z);

        visualStartPos = visual.localPosition;
        facingYaw = data.artFacesLeft ? 180f : 0f;

        if (spriteRenderer == null)
            spriteRenderer = visual.GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        StickToGround();
        AnimateVisual();
        AnimateSprite();
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

        isMoving = amount > 0 ? true : false;

        // Hop, scaled by speed so it settles back down when we stop.
        hopPhase = (hopPhase + Mathf.Abs(world.Speed) * data.hopsPerUnit * Time.deltaTime) % 1f;
        float hop = Mathf.Abs(Mathf.Sin(hopPhase * Mathf.PI)) * data.hopHeight * amount;
        visual.localPosition = visualStartPos + Vector3.up * hop;

        // Paper Mario flip: the flat sprite spins 180 on Y to turn around.
        // Plain MoveTowards (not MoveTowardsAngle) between 0 and 180 so it flips back the way it came.
        float rightYaw = data.artFacesLeft ? 180f : 0f;
        float leftYaw = 180f - rightYaw;
        if (world.MoveInput > 0f) facingYaw = Mathf.MoveTowards(facingYaw, rightYaw, data.turnSpeed * Time.deltaTime);
        if (world.MoveInput < 0f) facingYaw = Mathf.MoveTowards(facingYaw, leftYaw, data.turnSpeed * Time.deltaTime);

        // Lean slightly into the walk.
        lean = Mathf.SmoothDamp(lean, -speed01 * data.leanAngle, ref leanVelocity, data.leanSmoothTime);

        visual.rotation = Quaternion.Euler(0f, 0f, lean) * Quaternion.Euler(0f, facingYaw, 0f);
    }

    void AnimateSprite()
    {
        Sprite[] activeSprites = isMoving ? movingSprites : stationarySprites;

        if (activeSprites == null || activeSprites.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % activeSprites.Length;
            spriteRenderer.sprite = activeSprites[currentFrame];
        }
    }
}