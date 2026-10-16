using UnityEngine;

public class ScrollLayer : MonoBehaviour
{
    public WorldSpinner world;
    public float scrollFactor = 1f;
    public float loopLength = 60f;

    [Header("Pop Up")]
    [Tooltip("Objects within this distance (left/right) of the player stand fully upright.")]
    public float popupDistance = 10f;
    [Tooltip("Distance past popupDistance over which objects tip from upright (0) to flat (90).")]
    public float popupLength = 3f;

    void LateUpdate()
    {
        float dx = -world.Speed * scrollFactor * Time.deltaTime;
        float centerX = world.PlayerSpot.x;
        float half = loopLength * 0.5f;

        foreach (Transform child in transform)
        {
            Vector3 p = child.position;
            p.x += dx;

            if (p.x < centerX - half) p.x += loopLength;
            else if (p.x > centerX + half) p.x -= loopLength;

            child.position = p;

            float t = Mathf.InverseLerp(popupDistance, popupDistance + popupLength, Mathf.Abs(p.x - centerX));
            child.rotation = Quaternion.Euler(Mathf.SmoothStep(0f, 90f, t), 0f, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        if (world == null || world.data == null) return;

        float centerX = world.PlayerSpot.x;
        float half = loopLength * 0.5f;

        DrawPair(centerX, half, 10f, Color.magenta);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(Point(centerX - half, 0f), Point(centerX + half, 0f));

        DrawPair(centerX, popupDistance, 6f, Color.green);

        DrawPair(centerX, popupDistance + popupLength, 4f, Color.yellow);
    }

    void DrawPair(float centerX, float dist, float height, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(Point(centerX - dist, 0f), Point(centerX - dist, height));
        Gizmos.DrawLine(Point(centerX + dist, 0f), Point(centerX + dist, height));
    }

    Vector3 Point(float x, float y)
    {
        return new Vector3(x, transform.position.y + y, transform.position.z);
    }
}