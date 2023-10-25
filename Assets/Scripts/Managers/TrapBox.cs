using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBox : MonoBehaviour
{
    [SerializeField, Tag] string[] trapTags;
    [field: SerializeField, ReadOnly] public bool IsTouching { get; private set; } = false;


    public void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.tag;
        foreach (string s in trapTags)
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

