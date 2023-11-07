using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{   
    public void SwitchPlayers()
    {
        SystemManager.Get<PlayerManager>().Switch();
    }
}
