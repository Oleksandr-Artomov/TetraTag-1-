using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerManager : Manager
{
    Board board;
    [SerializeField] Player playerPrefab;
    [SerializeField] Transform playerSpawnPosition;
    [SerializeField] bool useKeyboardAsBoardPlayer;
    [HideInInspector] public UnityEvent OnAllPlayersSquished;
    [HideInInspector] public UnityEvent OnPlayerEscaped;

    int playersSquished;
    

    [SerializeField, DisplayInspector] List<Actor> actors = new List<Actor>();

    public void OnGameStart()
    {
        var devices = InputSystem.devices;

        playersSquished = 0;
        actors.Clear();
        board = FindObjectOfType<Board>();

        if (useKeyboardAsBoardPlayer) SetUp(devices, 0);
        else NormalStart(devices);
    }

    void NormalStart(ReadOnlyArray<InputDevice> devices)
    {
        int i = System.Array.FindIndex(devices.ToArray(), d => d is Gamepad);
        board.DeviceID = devices[i++].deviceId;
        SetUp(devices, i);   
    }

    void SetUp(ReadOnlyArray<InputDevice> devices, int i = 0)
    {
        actors.Add(board);
        for (; i < devices.Count; i++)
        {
            if (devices[i] is not Gamepad) continue;

            var player = Instantiate(playerPrefab);
            player.transform.position = playerSpawnPosition.position;
            player.DeviceID = devices[i].deviceId;
            actors.Add(player);
        }
    }

    public void Switch()
    {
        List<int> ids = new List<int>();


        foreach (var player in actors)
        {
            if (player.DeviceID != -1)
                ids.Add(player.DeviceID);
        }
            

        for (int i = 0, j = 0; i < actors.Count; i++)
        {
            if (useKeyboardAsBoardPlayer && actors[i] == board) continue;
            actors[i].DeviceID = ids[(j + 1) % ids.Count];
            ++j;
        }
            
    }

    public void OnPlayerSquished(Player player)
    {
        player.gameObject.SetActive(false);
        ++playersSquished;
        if (playersSquished == actors.Count - 1)
            OnAllPlayersSquished?.Invoke();

    }
   
    public void PlayerEscaped(Player player)
    {
        player.gameObject.SetActive(false);
        OnPlayerEscaped?.Invoke();
    }
}
