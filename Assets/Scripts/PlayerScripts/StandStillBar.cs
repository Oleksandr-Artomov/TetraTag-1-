using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandStillBar : MonoBehaviour
{
    [SerializeField] RectTransform content;
    public void OnGameStart()
    {
        SystemManager.Get<PlayerManager>().SetUpStandStillBars(content);
    }
}
