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
        Debug.Log("Number of actors: " + actors.Count);
    }

    public void Switch()
    {
        if (actors.Count <= 1)
            return;

        List<int> playerIDs = new List<int>();

        // Collect all player device IDs except the board
        foreach (var actor in actors)
        {
            if (actor != board && actor.DeviceID != -1)
            {
                playerIDs.Add(actor.DeviceID);
            }
        }

        if (playerIDs.Count < 2)
            return;

        // Rotate the player device IDs to switch roles
        int temp = playerIDs[playerIDs.Count - 1];
        for (int i = playerIDs.Count - 1; i > 0; i--)
        {
            playerIDs[i] = playerIDs[i - 1];
        }
        playerIDs[0] = temp;

        // Assign the rotated device IDs back to the players
        int playerIndex = 0;
        foreach (var actor in actors)
        {
            if (actor != board)
            {
                actor.DeviceID = playerIDs[playerIndex];
                playerIndex++;
            }
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








