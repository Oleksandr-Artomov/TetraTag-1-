using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameState
{ 
    Gameplay,
    Lobby,
}
public class GameManager : Manager
{
    [SerializeField] SceneReference lobbyScene;
    [SerializeField] SceneReference[] levels;
    [SerializeField, ReadOnly] int currentLevel;
    [SerializeField] GameState startingGamestate;
    [SerializeField, ReadOnly] GameState currentState;
    public GameState CurrentState { get => currentState; set => ChangeState(value); }

    public override void OnStart()
    {
        var playerManager = SystemManager.Get<PlayerManager>();

        playerManager.OnAllPlayersSquished.AddListener(OnBoardPlayerWin);
        playerManager.OnPlayerEscaped.AddListener(OnPlayersWin);

        CurrentState = startingGamestate;
    }

    void ChangeState(GameState state)
    {
        if (CurrentState == state) return;

        currentState = state;

        switch (CurrentState)
        {
            case GameState.Gameplay: break;
            case GameState.Lobby: break;
        }
    }

    public void LoadIntoGame()
    {
        CurrentState = GameState.Gameplay;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SwitchScene();
    }

    public override void OnEnd()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    public void OnBoardPlayerWin()
    {
        ++currentLevel;
        if (currentLevel >= levels.Length)
            currentLevel = 0;

        print("Board Player Wins");
        SwitchScene();
    }

    public void OnPlayersWin()
    {
        print("Runners Wins");
        SwitchScene();
    }

    void SwitchScene()
    {  
        SceneManager.LoadScene(levels[currentLevel].SceneName);
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (currentState != GameState.Gameplay) return;

        var playerManager = SystemManager.Get<PlayerManager>();

        if (playerManager != null)
        {
            playerManager.OnGameStart();
        }
        else
        {
            Debug.LogError("PlayerManager not found or not accessible.");
        }
    }
}

