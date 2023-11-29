using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lobby : MonoBehaviour
{
    [SerializeField] Transform playerSpawnPosition;
    public List<int> deviceIDs = new List<int>();
    public void FixedUpdate()
    {
        var devices = InputSystem.devices;
        var playerManager = SystemManager.Get<PlayerManager>();

        foreach (var device in devices)
        {
            if (deviceIDs.FindIndex(d => d == device.deviceId) != -1) continue;
            if (device is not Gamepad) continue;

            deviceIDs.Add(device.deviceId);
            var player = playerManager.CreatePlayer(device.deviceId);
            player.transform.position = playerSpawnPosition.position;
        }        
    }

    public void StartGame()
    {
        SystemManager.Get<GameManager>().LoadIntoGame();
    }
}
