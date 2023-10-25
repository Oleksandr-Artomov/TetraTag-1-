using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TarodevController
{
    public class PlayerInput : Player
    {
        FrameInput frameInput;
        public FrameInput Gather() => frameInput;

        private void Update()
        {
            var device = InputSystem.GetDeviceById(DeviceID);
            if (device is Gamepad gamepad) UpdateInputs(gamepad);

        }

        private void UpdateInputs(Gamepad gamepad)
        {
            Vector2 move;
            move = gamepad.leftStick.ReadValue();

            if (move.x == 0 && move.y == 0)
            {
                frameInput.Move.x = gamepad.dpad.left.isPressed ? -1 : 0;
                if (!gamepad.dpad.left.isPressed)
                    frameInput.Move.x = gamepad.dpad.right.isPressed ? 1 : 0;
            }
            else frameInput.Move = move;    

            frameInput.JumpDown = gamepad.aButton.wasPressedThisFrame;
            frameInput.JumpHeld = gamepad.aButton.isPressed;
            frameInput.DashDown = gamepad.xButton.wasPressedThisFrame;
        }
    }

    public struct FrameInput
    {
        public Vector2 Move;
        public bool JumpDown;
        public bool JumpHeld;
        public bool DashDown;
    }
}