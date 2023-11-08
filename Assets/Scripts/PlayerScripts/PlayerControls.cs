using System;
using UnityEngine.InputSystem;
using UnityEngine;
using MyBox;
using UnityEngine.InputSystem.LowLevel;
using Newtonsoft.Json.Linq;

[Serializable]
public class PlayerControls
{
    [Serializable]
    public struct ThisFrame
    {
        [ReadOnly] public bool rightPressed;    // Up instead of left
        [ReadOnly] public bool leftPressed;  // Down instead of right
        [ReadOnly] public bool jumpPressed;
        [ReadOnly] public bool dashPressed;

        [ReadOnly] public bool jumpReleased;
        [ReadOnly] public bool rightReleased;    // Up instead of left
        [ReadOnly] public bool leftReleased;  // Down instead of right
    }
    [ReadOnly] public bool isHoldingRight;     // Up instead of left
    [ReadOnly] public bool isHoldingLeft;   // Down instead of right
    [ReadOnly] public bool isHoldingJump;

    public GamepadButton
        dashButton,
        jumpButton;

    [ReadOnly] public ThisFrame thisFrame;
    [field: SerializeField, ReadOnly] public float TimeOfDash { get; private set; }
}

public static class InputHandler
{
    public static void HandleInputs(PlayerControls controls, int deviceID, ref bool canJump)
    {
        Gamepad gamepad = null;
        foreach (var device in InputSystem.devices)
        {
            if (device.deviceId == deviceID)
                if (device is Gamepad pad)
                {
                    gamepad = pad;
                    break;
                }
        }

        if (gamepad is null) return;

        controls.thisFrame.rightPressed = !controls.isHoldingRight && gamepad.dpad.right.wasPressedThisFrame || gamepad.leftStick.right.wasPressedThisFrame;   // Up instead of left
        controls.thisFrame.leftPressed = !controls.isHoldingLeft && gamepad.dpad.left.isPressed || gamepad.leftStick.left.isPressed; // Down instead of right

        controls.thisFrame.dashPressed = gamepad[controls.dashButton].wasPressedThisFrame;
        controls.thisFrame.jumpPressed = gamepad[controls.jumpButton].wasPressedThisFrame;

        controls.isHoldingRight = gamepad.dpad.right.isPressed || gamepad.leftStick.right.isPressed;   // Up instead of left
        controls.isHoldingLeft = gamepad.dpad.left.isPressed || gamepad.leftStick.left.isPressed; // Down instead of right
        controls.isHoldingJump = gamepad[controls.jumpButton].isPressed;

        if (controls.isHoldingRight && controls.thisFrame.leftPressed) controls.isHoldingRight = false;     // Up instead of left
        if (controls.isHoldingLeft && controls.thisFrame.rightPressed) controls.isHoldingLeft = false; // Down instead of right

        controls.thisFrame.jumpReleased = gamepad[controls.jumpButton].wasReleasedThisFrame;
        controls.thisFrame.rightReleased = gamepad.dpad.right.wasReleasedThisFrame || gamepad.leftStick.right.wasPressedThisFrame;   // Up instead of left
        controls.thisFrame.leftReleased = gamepad.dpad.left.wasReleasedThisFrame || gamepad.leftStick.left.wasPressedThisFrame; // Down instead of right

        if (controls.thisFrame.jumpReleased)
            canJump = true;
    }
}