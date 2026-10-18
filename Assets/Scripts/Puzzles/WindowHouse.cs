using System.Collections;
using UnityEngine;

public class WindowHouse : Interactable
{
    public WindowPuzzle puzzle;
    public CameraFollow cam;
    public Transform zoomTarget;
    public Vector3 zoomOffset = new Vector3(0f, 0.5f, -5f);
    public float solvedExitDelay = 1f;

    public bool Engaged { get; private set; }

    private void Awake()
    {
        cam = Camera.main.GetComponent<CameraFollow>();
    }

    public override void Interact(PlayerInteractor player)
    {
        player.RunSequence(Inspect());
    }

    IEnumerator Inspect()
    {
        Transform oldTarget = cam.target;
        Vector3 oldOffset = cam.offset;
        bool oldCursor = Cursor.visible;

        cam.target = zoomTarget != null ? zoomTarget : transform;
        cam.offset = zoomOffset;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Engaged = true;

        bool wasSolved = puzzle.Solved;
        do yield return null;
        while (!PlayerInteractor.InteractDown() && (wasSolved || !puzzle.Solved));

        CursorManager.Instance.SetHover(false);
        Engaged = false;
        if (!wasSolved && puzzle.Solved) yield return new WaitForSeconds(solvedExitDelay);

        Engaged = false;
        cam.target = oldTarget;
        cam.offset = oldOffset;
        Cursor.visible = oldCursor;
    }
}
