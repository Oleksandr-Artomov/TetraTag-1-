using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lobby : MonoBehaviour
{
    [SerializeField] Transform playerSpawnPosition;
    [SerializeField] Player playerPrefab;
    public List<int> deviceIDs = new List<int>();
    public void FixedUpdate()
    {
        var devices = InputSystem.devices;

        foreach (var device in devices)
        {
            if (deviceIDs.FindIndex(d => d == device.deviceId) != -1) continue;
            if (device is not Gamepad) continue;

            deviceIDs.Add(device.deviceId);

            var player = Instantiate(playerPrefab);
            player.transform.position = playerSpawnPosition.position;
            player.DeviceID = device.deviceId;
        }        
    }

    public void StartGame()
    {
        SystemManager.Get<GameManager>().LoadIntoGame();
    }
}
