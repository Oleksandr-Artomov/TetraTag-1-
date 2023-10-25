using MyBox;
using System;
using TarodevController;
using UnityEngine;
using UnityEngine.Events;
public class Player : Actor
{
    [SerializeField] SquishBox[] squishBoxes;
    float waitTime;

    private void Start()
    {
        waitTime = Time.time;
    }
    private void FixedUpdate()
    {
        if (Time.time - waitTime < 0.75f) return;
        if (!Array.TrueForAll(squishBoxes, s => s.IsTouching)) return;

        SystemManager.Get<PlayerManager>().OnPlayerSquished(this);
    }

    /*private void OnCollisionEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WinTag"))
        {
            SystemManager.Get<PlayerManager>().OnPlayerEscaped?.Invoke();
        }
    }*/
    private void OnDestroy()
    {
        print("Here");
    }
}
