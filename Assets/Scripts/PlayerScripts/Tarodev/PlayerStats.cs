using System;
using UnityEngine;

namespace TarodevController
{
    [CreateAssetMenu]
    public class PlayerStats : ScriptableObject
    {
        // Setup
        [Header("Setup")] public LayerMask PlayerLayer;
        public CharacterSize CharacterSize;

        // Controller Setup
        [Header("Controller Setup"), Space] public float VerticalDeadZoneThreshold = 0.3f;
        public double HorizontalDeadZoneThreshold = 0.1f;

        // Movement
        [Header("Movement"), Space] public float BaseSpeed = 9;
        public float Acceleration = 50;
        public float Friction = 30;
        public float AirFrictionMultiplier = 0.5f;
        public float DirectionCorrectionMultiplier = 3f;
        public float MaxWalkableSlope = 50;

        // Jump
        [Header("Jump"), Space] public float ExtraConstantGravity = 40;
        public float BufferedJumpTime = 0.15f;
        public float CoyoteTime = 0.15f;
        public float JumpPower = 20;
        public float EndJumpEarlyExtraForceMultiplier = 3;
        public int MaxAirJumps = 1;

        // Dash
        [Header("Dash"), Space] public bool AllowDash = true;
        public float DashVelocity = 50;
        public float DashDuration = 0.2f;
        public float DashCooldown = 1.5f;
        public float DashEndHorizontalMultiplier = 0.5f;

        // Dash
        [Header("Crouch"), Space] public bool AllowCrouching;
        public float CrouchSlowDownTime = 0.5f;
        public float CrouchSpeedModifier = 0.5f;

        // Walls
        [Header("Walls"), Space] public bool AllowWalls;
        public LayerMask ClimbableLayer;
        public float WallJumpInputLossTime = 0.5f;
        public bool RequireInputPush;
        public Vector2 WallJumpPower = new(25, 15);
        public Vector2 WallPushPower = new(15, 10);
        public float WallClimbSpeed = 5;
        public float WallFallAcceleration = 20;
        public float WallPopForce = 10;
        public float WallCoyoteTime = 0.3f;
        public float WallDetectorRange = 0.1f;

        // Ladders
        [Header("Ladders"), Space] public bool AllowLadders;
        public double LadderCooldownTime = 0.15f;
        public bool AutoAttachToLadders = true;
        public bool SnapToLadders = true;
        public LayerMask LadderLayer;
        public float LadderSnapTime = 0.02f;
        public float LadderPopForce = 10;
        public float LadderClimbSpeed = 8;
        public float LadderSlideSpeed = 12;
        public float LadderShimmySpeedMultiplier = 0.5f;

        // Moving Platforms
        [Header("Moving Platforms"), Space] public float NegativeYVelocityNegation = 0.2f;
        public float ExternalVelocityDecayRate = 0.1f;

        private void OnValidate()
        {
            var potentialPlayer = FindObjectsOfType<PlayerController>();
            foreach (var player in potentialPlayer)
            {
                player.OnValidate();
            }
        }
    }

    [Serializable]
    public class CharacterSize
    {
        public const float STEP_BUFFER = 0.05f;

        [Range(0.5f, 20), Tooltip("How tall you are. This includes a collider and your step height.")]
        public float Height = 1.8f;

        [Range(0.1f, 0.8f), Tooltip("Affects your collider radius and is directly related to your height")]
        public float ChonkPercentage = .45f;

        [Range(STEP_BUFFER, 15), Tooltip("Step height allows you to step over rough terrain like steps and rocks.")]
        public float StepHeight = 0.5f;

        [Range(0.1f, 0.9f),
         Tooltip(
             "A percentage of your height stat which determines your height while crouching. A smaller crouch requires more step height sacrifice")]
        public float CrouchHeight = 0.5f;
        
        public GeneratedCharacterSize GenerateCharacterSize()
        {
            const float MIN_CROUCHING_STEP_HEIGHT = 0.02f;

            var s = new GeneratedCharacterSize();
            s.Height = Height;
            s.StepHeight = StepHeight;

            s.StandingColliderSize = new Vector2((s.Height * ChonkPercentage), s.Height - s.StepHeight);
            s.StandingColliderCenter = new Vector2(0, s.Height - s.StandingColliderSize.y / 2);

            do
            {
                s.CrouchingHeight = s.Height * CrouchHeight;

                // Choose the biggest crouch collider height we can use
                s.CrouchColliderSize = new Vector2(s.StandingColliderSize.x, Mathf.Max(s.StandingColliderSize.x, s.CrouchingHeight - s.StepHeight));

                s.CrouchingColliderCenter = new Vector2(0, s.CrouchingHeight - s.CrouchColliderSize.y / 2);

                s.CrouchingStepHeight = s.CrouchingHeight - s.CrouchColliderSize.y;

                if (s.CrouchingStepHeight < MIN_CROUCHING_STEP_HEIGHT) CrouchHeight += 0.05f;
            } while (s.CrouchingStepHeight < MIN_CROUCHING_STEP_HEIGHT);

            return s;
        }
    }

    public struct GeneratedCharacterSize
    {
        // Standing
        public float Height;
        public float StepHeight;
        public Vector2 StandingColliderSize;
        public Vector2 StandingColliderCenter;
        
        // Crouching
        public Vector2 CrouchColliderSize;
        public float CrouchingHeight;
        public Vector2 CrouchingColliderCenter;
        public float CrouchingStepHeight;
    }
}