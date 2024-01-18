using MyBox;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player : Actor
{
    [SerializeField] PlayerControls controls;
    [SerializeField] MovementValues movementValues;
    [SerializeField] SquishBox[] squishBoxes;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float timeToFill;
    [SerializeField, ReadOnly] bool escaped;
   
    GameManager gameManager;
    public Image standStillBar;
    float waitTime;
    float fillTimer = 0;
    [SerializeField, Range(1f, 5f)] float fillRate = 1f;
    
    

    private void Start()
    {
        waitTime = Time.time;
        fillTimer = 0;
        escaped = false;
        gameManager = SystemManager.Get<GameManager>();
    }
    private void FixedUpdate()
    {
        Movement.FixedUpdate(controls, ref movementValues);

        if (Time.time - waitTime < 0.75f) return;
        if (squishBoxes.Length == 0) return;

        if (!Array.TrueForAll(squishBoxes, s => s.IsTouching)) return;

        SystemManager.Get<PlayerManager>().OnPlayerDeath(this);
    }
    private void Update()
    {
        InputHandler.HandleInputs(controls, DeviceID, ref movementValues.jumpValues.canJump);
        Movement.Update(controls, ref movementValues);

        if (gameManager.CurrentState == GameState.Gameplay)
        {
            UpdateStandStill();
        }
    }

    void UpdateStandStill()
    {
        if (Time.time - waitTime < 0.75f) return;
        bool isMoving = movementValues.rigidbody.velocity.sqrMagnitude > 0.05f;

        if (!isMoving)
            fillTimer += Time.deltaTime * fillRate;
        else 
            fillTimer -= Time.deltaTime;

        standStillBar.fillAmount = Mathf.Clamp01(fillTimer / timeToFill);

        if (!escaped && standStillBar.fillAmount >= 1)
        {
            escaped = true;
            SystemManager.Get<PlayerManager>().OnPlayerWin(this);
        }
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;   
    }
}
