using MyBox;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Player : Actor
{
    [SerializeField] PlayerControls controls;
    [SerializeField] MovementValues movementValues;
    [SerializeField] SquishBox[] squishBoxes;
    [SerializeField] SpriteRenderer spriteRenderer;
    float waitTime;
    
    

    private void Start()
    {
        waitTime = Time.time;
    }
    private void FixedUpdate()
    {
        Movement.FixedUpdate(controls, ref movementValues);

        if (Time.time - waitTime < 0.75f) return;
        if (squishBoxes.Length == 0) return;

        if (!Array.TrueForAll(squishBoxes, s => s.IsTouching)) return;

        SystemManager.Get<PlayerManager>().OnPlayerSquished(this);
    }
    private void Update()
    {
        InputHandler.HandleInputs(controls, DeviceID, ref movementValues.jumpValues.canJump);
        Movement.Update(controls, ref movementValues);
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;   
    }
}
