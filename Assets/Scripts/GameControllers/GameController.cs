using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum GameState { Starting, Running, Ended, Disconnected };

    private float _startTime;
    public GameState state;
    private bool gameOver;
    private float passedTime = 0f;
    private List<NetworkPlayerController> _networkPlayerControllers = new List<NetworkPlayerController>();
    private XmlDocument settingsInfo = new XMLReader("GameSettings.xml").GetXML();

    private float _currentMinionSpawn = 10f;
    private float _spawnTime = 30f;
    private float _spawnTimer = 0f;

    public int pointsTeam1 = 0;
    public int pointsTeam2 = 0;

    public bool musicPlayed = false;

    [SerializeField]
    public Color[] teamColors = new Color[3];

    private string message;

    public string Message
    {
        get { return message; }
    }

    void Awake()
    {
        Team.teamColors = teamColors;
    }

    public NetworkPlayerController GetNetworkPlayerController(NetworkPlayer player)
    {
        return _networkPlayerControllers.FirstOrDefault(controller => controller.networkPlayer == player);
    }

    public float GameTime
    {
        get { return Time.time - _startTime; }
    }

    public float FirstMinionSpawn
    {
        set { _currentMinionSpawn = value; }
        get { return _currentMinionSpawn; }
    }

    public float SpawnTime
    {
        set { _spawnTime = value; }
        get { return _spawnTime; }
    }

    void OnPlayerDisconnected()
    {
        if (state == GameState.Running)
            SetGameState(GameState.Disconnected);
    }

    void OnDisconnectedFromServer()
    {
        if (state == GameState.Running)
            SetGameState(GameState.Disconnected);
    }


    private void YouWon()
    {
        message = "You won!";
        GameObject.FindGameObjectWithTag(Tags.camera).transform.FindChild("sounds_camera").FindChild("sounds_Vocal")
                         .GetComponent<AudioLibrary>()
                         .StartSound("HuelYouWin-alt");
    }

    private void YouLost()
    {
        message = "Sorry man! Your opponent was better. Maybe you are too weak.";
        GameObject.FindGameObjectWithTag(Tags.camera).transform.FindChild("sounds_camera").FindChild("sounds_Vocal")
                 .GetComponent<AudioLibrary>()
                 .StartSound("HuelYouLose");
    }

    void Update()
    {
        if (state == GameState.Disconnected)
        {
            message = "Your opponent disconneced.";

            passedTime += Time.deltaTime;

            if (passedTime >= 3)
            {
                Application.Quit();
            }
        }
        else if (gameOver)
        {
            if (pointsTeam1 > pointsTeam2)
                if (!Network.isServer)
                    YouWon();
                else
                    YouLost();
            else
                if (Network.isServer)
                    YouWon();
                else
                    YouLost();

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
            if (GameObject.FindGameObjectWithTag(Tags.minionManager) == null) return;
            if (GameObject.FindGameObjectWithTag(Tags.minionManager).GetComponent<MinionManager>() == null) return;
            GameObject.FindGameObjectWithTag(Tags.minionManager).GetComponent<MinionManager>().SpawnMinions();
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
        if (teamID == (int)Team.TeamIdentifier.Team1)
            pointsTeam1++;
        else
            pointsTeam2++;

        if (pointsTeam1 == 2 || pointsTeam2 == 2)
            SetGameState(GameState.Ended);

    }
}
