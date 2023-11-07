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
        [ReadOnly] public bool leftPressed;
        [ReadOnly] public bool rightPressed;
        [ReadOnly] public bool jumpPressed;
        [ReadOnly] public bool dashPressed;

        [ReadOnly] public bool jumpReleased;
        [ReadOnly] public bool leftReleased;
        [ReadOnly] public bool rightReleased;

    }
    [ReadOnly] public bool isHoldingLeft;
    [ReadOnly] public bool isHoldingRight;
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

        controls.thisFrame.leftPressed = !controls.isHoldingLeft && gamepad.dpad.left.wasPressedThisFrame || gamepad.leftStick.left.wasPressedThisFrame;
        controls.thisFrame.rightPressed = !controls.isHoldingRight && gamepad.dpad.right.isPressed || gamepad.leftStick.right.isPressed;

        controls.thisFrame.dashPressed = gamepad[controls.dashButton].wasPressedThisFrame;
        controls.thisFrame.jumpPressed = gamepad[controls.jumpButton].wasPressedThisFrame;

        controls.isHoldingLeft = gamepad.dpad.left.isPressed || gamepad.leftStick.left.isPressed;
        controls.isHoldingRight = gamepad.dpad.right.isPressed || gamepad.leftStick.right.isPressed;
        controls.isHoldingJump = gamepad[controls.jumpButton].isPressed;

        if (controls.isHoldingLeft && controls.thisFrame.rightPressed) controls.isHoldingLeft = false;
        if (controls.isHoldingRight && controls.thisFrame.leftPressed) controls.isHoldingRight = false;

        controls.thisFrame.jumpReleased = gamepad[controls.jumpButton].wasReleasedThisFrame;
        controls.thisFrame.leftReleased = gamepad.dpad.left.wasReleasedThisFrame || gamepad.leftStick.left.wasPressedThisFrame;
        controls.thisFrame.rightReleased = gamepad.dpad.right.wasReleasedThisFrame || gamepad.leftStick.right.wasPressedThisFrame;

        if (controls.thisFrame.jumpReleased)
            canJump = true;
    }
}