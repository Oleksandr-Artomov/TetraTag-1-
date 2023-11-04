using MyBox;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IsGrounded : MonoBehaviour
{
    const int SIZE = 10;

    [SerializeField] BoxCollider2D groundedCollider;
    RaycastHit2D[] raycastHits = new RaycastHit2D[SIZE];
    List<Collider2D> colliders = new(SIZE);
    [ReadOnly] public bool isGrounded;

    private void Start()
    {
        for (int i = 0; i < SIZE; i++)
            colliders.Add(null);
    }
    public void FixedUpdate()
    {
        Bounds bounds = groundedCollider.bounds;
        int hitCount = Physics2D.BoxCastNonAlloc(bounds.center, bounds.size, 0f, Vector2.down, raycastHits, bounds.size.y);

        Clear();
        if (hitCount == 0)
        {
            return;
        }

        isGrounded = false;
        for (int i = 0; i < hitCount; i++)
        {
            colliders[i] = raycastHits[i].collider;
            if (!colliders[i].CompareTag("Board")) continue;
            isGrounded = true;
            return;
        }
    }

    void Clear()
    {
        for (int i = 0; i < SIZE; i++)
            colliders[i] = null;
    }

    public bool IsCollidingWith(Collider2D collider)
    {
        return colliders.Contains(collider);
    }
}
