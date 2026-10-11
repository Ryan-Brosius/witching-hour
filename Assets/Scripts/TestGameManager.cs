using UnityEngine;
using UnityEngine.InputSystem;

public class TestGameManager : MonoBehaviour
{
    public WorldSpinner world;

    void Update()
    {
        float input = 0f;

        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input += 1f;
        }

        Gamepad pad = Gamepad.current;
        if (pad != null && input == 0f)
            input = pad.leftStick.x.ReadValue();

        world.SetMoveInput(input);
    }
}
