using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishBox : MonoBehaviour
{
    [SerializeField, Tag] string[] squishTags;
    [field: SerializeField, ReadOnly] public bool IsTouching { get; private set; } = false;

    
    public void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.tag;
        foreach (string s in squishTags)
        {
            if (s == tag)
            {
                IsTouching = true;
                return;
            }      
        }

        IsTouching = false;
    }
}
