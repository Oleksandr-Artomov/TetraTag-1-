using UnityEngine;
using MyBox;
using System;

[System.Serializable]
public struct JumpValues
{
    public enum Type
    {
        None = 0,
        Upright,
    }
    public enum Perk
    {
        None = 0,
        Double,
    }

    [Serializable]
    public struct Double
    {
        [Range(0.5f, 1f)] public float heightPercentage;
        [ReadOnly] public bool secondJumpAvailible;
        [ReadOnly] public bool secondJumpInUse;
    }


    public float jumpForce;
    public float maxAirTime;
    [Range(0.05f, 0.35f)] 
    public float jumpBuffer;
    [Range(0.5f, 1f)]
    public float slidingJumpForceReduction;
    [Range(0.08f, 0.3f)]
    public float fallGravityScale;
    [Range(0f, 0.99f)]
    public float turnAroundMomentumDeduction;
    public Perk jumpPerk;

    public Double doubleJump;

    [ReadOnly] public bool validJumpPress;
    [ReadOnly] public Type jumpType;
    [ReadOnly] public float airTime;
    [ReadOnly] public float bufferTimer;
    [ReadOnly] public bool canJump;
    [ReadOnly] public bool appliedFallVelocity;
}
public static class Jump
{
    public static void Update(ref JumpValues values, bool jumpPressed)
    {
        if (jumpPressed)
        {
            values.validJumpPress = true;
            values.bufferTimer = values.jumpBuffer;
        }
        else if (values.bufferTimer > 0f)
        {
            values.bufferTimer -= Time.deltaTime;
            if (values.bufferTimer < 0f)
                values.validJumpPress = false;
        } 
    }
    public static void DoJump(ref JumpValues values, Rigidbody2D rigidbody, bool isGrounded, bool isHoldingJump)
    {
        switch (values.jumpPerk)
        {
            case JumpValues.Perk.Double:
            case JumpValues.Perk.None:
                DoSingleJump(ref values, isGrounded, isHoldingJump, rigidbody);
                break;
        }
    }

    public static void ValidateJump(ref JumpValues values, PlayerControls controls, bool isGrounded)
    {
        switch (values.jumpPerk)
        {
            case JumpValues.Perk.None:
                ValidateSingleJump(ref values, controls, isGrounded);
                break;
            case JumpValues.Perk.Double:
                ValidateDoubleJump(ref values, controls, isGrounded);
                break;
        }
    }

    static void ValidateSingleJump(ref JumpValues values, PlayerControls controls, bool isGrounded)
    {
        if (values.validJumpPress && values.canJump && values.jumpType == JumpValues.Type.None)
        {
            values.appliedFallVelocity = false;
            values.jumpType = JumpValues.Type.Upright;
        }
        if (isGrounded)
        {
            if (!controls.isHoldingJump)
            {
                values.airTime = 0;
                values.jumpType = JumpValues.Type.None;
            }
        }
        else
        {
            if (controls.isHoldingJump) values.airTime += Time.deltaTime;
            else values.airTime = values.maxAirTime + 1;
        }
    }

    static void ValidateDoubleJump(ref JumpValues values, PlayerControls controls, bool isGrounded)
    {
        if (values.validJumpPress && values.canJump && values.jumpType == JumpValues.Type.None)
        {
            values.appliedFallVelocity = false;
            values.jumpType = JumpValues.Type.Upright;
        }

        if (isGrounded)
        {
            if (!controls.isHoldingJump)
            {
                values.airTime = 0;
                values.jumpType = JumpValues.Type.None;
                values.doubleJump.secondJumpAvailible = true;
                values.doubleJump.secondJumpInUse = false;
            }
        }
        else
        {
            if (controls.isHoldingJump) values.airTime += Time.deltaTime;
            else if (controls.thisFrame.jumpReleased && values.doubleJump.secondJumpAvailible)
            {
                values.airTime = 0;
                values.doubleJump.secondJumpAvailible = false;
            }   
            else if (values.doubleJump.secondJumpInUse)
            {
                values.airTime = values.maxAirTime + 1;
            }

            if (controls.thisFrame.jumpPressed && values.doubleJump.secondJumpAvailible)
            {
                values.doubleJump.secondJumpInUse = true;
                values.doubleJump.secondJumpAvailible = false;
                values.airTime = 0;
            }

            if (!values.doubleJump.secondJumpInUse && !values.doubleJump.secondJumpAvailible && controls.thisFrame.jumpPressed)
            {
                values.doubleJump.secondJumpInUse = true;
                values.airTime = values.maxAirTime * (1f - values.doubleJump.heightPercentage);
            }
        }
    }


    static void DoSingleJump(ref JumpValues values, bool isGrounded, bool isHoldingJump, Rigidbody2D rigidbody)
    {
        if (!isHoldingJump) return;

        //if the player hasn't let go of jump before touhcing the ground exit
        if (!values.canJump && rigidbody.velocity.y <= 0) return;
        if (values.airTime >= values.maxAirTime) return;

        //if the player is grounded and hasn't reached the air time threshold increase his upwards veloicty at a diminishing rate
        if (isGrounded || values.airTime < values.maxAirTime)
            rigidbody.velocity = rigidbody.velocity.SetY(values.jumpForce);
    }

    

    public static void ResetJumpParameters(ref JumpValues values)
    {
        values.airTime = 0;
        values.canJump = true;

        values.airTime = 0;
        values.jumpType = JumpValues.Type.None;
        values.doubleJump.secondJumpAvailible = true;
        values.doubleJump.secondJumpInUse = false;

        values.appliedFallVelocity = false;
    }
}
