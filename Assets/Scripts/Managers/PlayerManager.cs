using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using System.Linq;

public class PlayerManager : Manager
{
    Board board;
    [SerializeField] Player playerPrefab;
    [SerializeField] Transform playerSpawnPosition;
    [SerializeField] bool useKeyboardAsBoardPlayer;
    [HideInInspector] public UnityEvent OnAllPlayersSquished;
    [HideInInspector] public UnityEvent OnPlayerEscaped;

    static Dictionary<int, Color> playerColors;
    Color[] colors = new Color[4]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    int playersSquished;

    int round = 0;

    [SerializeField, DisplayInspector] List<Actor> actors = new List<Actor>();
    static PlayerManager()
    {
        playerColors = new();
    }
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

        round++;    
        for(int j = 0; j < round; j++)
        {
            Switch();
        } 
    }
    public Player CreatePlayer(int deviceID)
    {
        var player = Instantiate(playerPrefab);
        player.transform.position = playerSpawnPosition.position;
        player.DeviceID = deviceID;


        var color = Color.white;
        if (playerColors.ContainsKey(deviceID))
        {
            color = playerColors[deviceID];
        }
        else
        {
            color = colors.GetRandom();

            //Get A Color That Is Not Being Used
            var collection = playerColors.Values;
            while (collection.Contains(color))
                color = colors.GetRandom();

            playerColors.Add(deviceID, color);
        }

        player.SetColor(color);
        return player;
    }
    void SetUp(ReadOnlyArray<InputDevice> devices, int i = 0)
    {
        actors.Add(board);
        for (; i < devices.Count; i++)
        {
            if (devices[i] is not Gamepad) continue;
            actors.Add(CreatePlayer(devices[i].deviceId));
        }

        foreach (var item in playerColors)
            print(item.Key + " - " + item.Value);

        Debug.Log("Number of actors: " + actors.Count);
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

            if (actors[i] is Player player)
            {
                player.SetColor(playerColors[player.DeviceID]);
            }
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








