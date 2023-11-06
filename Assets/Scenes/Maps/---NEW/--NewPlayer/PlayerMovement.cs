using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;

[Serializable]
public struct MovementValues
{
    public JumpValues jumpValues;

    [ReadOnly] public bool isFacingLeft;
    public bool IsGrounded { get => grounded.isGrounded; }
    public float timeOfTurnAround;
    public float moveSpeed;
    public float maxSpeed;
    [Range(0f, 1f)]
    public float airResistance;

    public Rigidbody2D rigidbody;
    public SpriteRenderer spriteRenderer;
    public IsGrounded grounded;
}

public static class Movement
{
    public enum TurnDirection
    {
        None,
        Left,
        Right,
    }

    public static void FixedUpdate(PlayerControls controls, ref MovementValues values)
    {
        Jump.DoJump(ref values.jumpValues, values.rigidbody, values.IsGrounded, controls.isHoldingJump);
        Move(ref values, controls);
    }

    public static void Update(PlayerControls controls, ref MovementValues values)
    {
        TurnAround(controls, ref values);
        Jump.Update(ref values.jumpValues, controls.thisFrame.jumpPressed);
        Jump.ValidateJump(ref values.jumpValues, controls, values.IsGrounded);
    }

    private static void TurnAround(PlayerControls controls, ref MovementValues values)
    {
        TurnDirection direction = TurnDirection.None;

        if (controls.thisFrame.leftPressed ||
            controls.thisFrame.rightReleased && controls.isHoldingLeft)
        {
            direction = TurnDirection.Left;
        }
        else if (controls.thisFrame.rightPressed ||
            controls.thisFrame.leftReleased && controls.isHoldingRight)
        {
            direction = TurnDirection.Right;
        }

        if (direction == TurnDirection.None) return;
        FaceDirection(ref values, direction);
    }

    public static void FaceDirection(ref MovementValues values, TurnDirection direction)
    {
        if (direction == TurnDirection.None) return;
        bool left = direction == TurnDirection.Left;
        values.isFacingLeft = left;
        values.spriteRenderer.flipX = left;
    }

    private static void Move(ref MovementValues values, PlayerControls controls)
    {
        Vector2 currentVelocity = values.rigidbody.velocity;

        // Calculate horizontal movement
        float horizontalInput = controls.isHoldingRight ? 1f : (controls.isHoldingLeft ? -1f : 0f);
        float targetSpeed = horizontalInput * values.moveSpeed;

        // Apply air resistance if not grounded
        if (!values.IsGrounded)
        {
            targetSpeed *= (1 - values.airResistance);
        }

        // Smoothly adjust the horizontal velocity
        float smoothTime = 0.1f; // Adjust this value as needed
        currentVelocity.x = Mathf.SmoothDamp(currentVelocity.x, targetSpeed, ref currentVelocity.x, smoothTime);

        // Limit the maximum speed
        currentVelocity.x = Mathf.Clamp(currentVelocity.x, -values.maxSpeed, values.maxSpeed);

        // Set the new velocity
        values.rigidbody.velocity = currentVelocity;
    }

}
