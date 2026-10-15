using UnityEngine;

public class ScrollLayer : MonoBehaviour
{
    public WorldSpinner world;
    [Tooltip("How many times this layer loops per full world spin. Higher = faster scroll.")]
    public int loopsPerSpin = 1;
    public float loopLength = 60f;

    float[] startX;

    void Start()
    {
        startX = new float[transform.childCount];
        for (int i = 0; i < startX.Length; i++)
            startX[i] = transform.GetChild(i).position.x;
    }

    void LateUpdate()
    {
        float scroll = -world.Angle / 360f * loopsPerSpin * loopLength;
        float centerX = world.PlayerSpot.x;
        float half = loopLength * 0.5f;

        for (int i = 0; i < startX.Length; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 p = child.position;
            p.x = centerX + Mathf.Repeat(startX[i] + scroll - centerX + half, loopLength) - half;
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