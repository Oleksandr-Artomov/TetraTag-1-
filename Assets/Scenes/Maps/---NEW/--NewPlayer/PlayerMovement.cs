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
    public float airResistenace;

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
        Vector2 current = values.rigidbody.transform.position;
        Vector2 currentVelocity = values.rigidbody.velocity;

        float speed = 0;
        if (controls.isHoldingRight || controls.isHoldingLeft)
        {
            speed = values.moveSpeed;
        }


        if (controls.isHoldingLeft) speed *= -1f;
        speed *= Time.deltaTime;

        if (!values.IsGrounded)
        {
            speed *= (1 - values.airResistenace);
        }


        Vector2.SmoothDamp(current, current.OffsetX(speed), ref currentVelocity, Time.deltaTime);
        currentVelocity.x = Mathf.Min(Mathf.Abs(currentVelocity.x), values.maxSpeed);
        if (values.isFacingLeft) currentVelocity.x *= -1f;
        values.rigidbody.velocity = values.rigidbody.velocity.SetX(currentVelocity.x);
    }
}
