using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum GameState { Starting, Running, Ended, Disconnected };

    private float _startTime;
    public GameState state;
    private bool gameOver;
    private float passedTime = 0f;
    private List<NetworkPlayerController> _networkPlayerControllers = new List<NetworkPlayerController>();

    private float _currentMinionSpawn = 10f;
    private float _spawnTime = 30f;
    private float _spawnTimer = 0f;

    public int pointsTeam1 = 0;
    public int pointsTeam2 = 0;

    public NetworkPlayerController GetNetworkPlayerController(NetworkPlayer player)
    {
        foreach (NetworkPlayerController controller in _networkPlayerControllers)
        {
            if (controller.networkPlayer == player)
            {
                return controller;
            }
        }
        return null;
    }

    public float GameTime
    {
        get { return Time.time - _startTime; }
    }

    public float FirstMinionSpawn
    {
        set { _currentMinionSpawn = value; }
        get { return _currentMinionSpawn;  }
    }

    public float SpawnTime
    {
        set { _spawnTime = value; }
        get { return _spawnTime; }
    }

    void OnPlayerDisconnected()
    {
        SetGameState(GameState.Disconnected);
    }

    void OnDisconnectedFromServer()
    {
        SetGameState(GameState.Disconnected);
    }

    void OnGUI()
    {
        if (state == GameState.Disconnected)
        {
            GUI.TextArea(new Rect(Screen.width * 0.5f, Screen.height * 0.5f, Screen.width * 0.2f, Screen.height * 0.1f),
                         "A player has been disconnected from the Game");
        }
        else if(gameOver)
        {
            if (pointsTeam1 > pointsTeam2)
                GUI.TextArea(new Rect(Screen.width * 0.5f, Screen.height * 0.5f, Screen.width * 0.2f, Screen.height * 0.1f),
                        "Team 1 won the Round");
            else
                GUI.TextArea(new Rect(Screen.width * 0.5f, Screen.height * 0.5f, Screen.width * 0.2f, Screen.height * 0.1f),
                        "Team 2 won the Round");
        }
    }

    void Update()
    {
        if (state == GameState.Disconnected)
        {
            passedTime += Time.deltaTime;

            if (passedTime >= 3)
            {
                Application.Quit();
            }
        }
        else if (gameOver)
        {
            passedTime += Time.deltaTime;

            if (passedTime >= 15)
            {
                Application.Quit();
            }
        }

        if (state == GameState.Running)
            _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= _currentMinionSpawn)
        {
            _currentMinionSpawn = _spawnTime;
            _spawnTimer = 0;
            GameObject.FindGameObjectWithTag(Tags.localPlayerController).GetComponent<LocalPlayerController>().SpawnMinions();
        }
    }

    [RPC]
    void AddNetworkPlayerController(int playerID, string playerName, int Team, NetworkPlayer networkPlayer)
    {
        _networkPlayerControllers.Add(new NetworkPlayerController(playerID, playerName, Team, networkPlayer));
    }

    [RPC]
    void SetGameState(int state)
    {
        SetGameState((GameState)state);
    }

    void SetGameState(GameState state)
    {
        this.state = state;
        if (this.state == GameState.Running)
        {
            GetComponent<GameSettings>().Init();
            _startTime = Time.time;
        }

        if (this.state == GameState.Ended)
        {
            gameOver = true;
        }
    }

    [RPC]
    public void IncreaseTeamPoints(int teamID)
    {
        if (teamID == (int) Team.TeamIdentifier.Team1)
            pointsTeam1++;
        else
            pointsTeam2++;

        if (pointsTeam1 == 2 || pointsTeam2 == 2)
            SetGameState(GameState.Ended);

    }
}
