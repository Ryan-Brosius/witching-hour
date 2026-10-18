using UnityEngine;
using UnityEngine.InputSystem;

// Test only: press T to send the camera to a point, T again to bring it back.
public class CameraMoveTest : MonoBehaviour
{
    public CameraFollow cam;
    public Transform point;

    Transform oldTarget;
    Vector3 oldOffset;
    bool atPoint;

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null || !kb.tKey.wasPressedThisFrame) return;

        if (!atPoint)
        {
            oldTarget = cam.target;
            oldOffset = cam.offset;
            cam.target = point;
            cam.offset = Vector3.zero;
        }
        else
        {
            cam.target = oldTarget;
            cam.offset = oldOffset;
        }
        atPoint = !atPoint;
    }
}
