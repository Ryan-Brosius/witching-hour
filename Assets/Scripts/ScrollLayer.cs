using UnityEngine;

public class ScrollLayer : MonoBehaviour
{
    public WorldSpinner world;
    public float scrollFactor = 1f;
    public float loopLength = 60f;

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
        }
    }

    private void OnDrawGizmos()
    {
        if (world == null || world.data == null) return;

        float centerX = world.PlayerSpot.x;
        float half = loopLength * 0.5f;
        Vector3 y = Vector3.up * 10f;

        Gizmos.color = Color.magenta;
        Vector3 left = new Vector3(centerX - half, transform.position.y, transform.position.z);
        Vector3 right = new Vector3(centerX + half, transform.position.y, transform.position.z);
        Gizmos.DrawLine(left, left + y);
        Gizmos.DrawLine(right, right + y);
        Gizmos.DrawLine(left, right);
    }
}
