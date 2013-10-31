using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class NetworkManager : MonoBehaviour
{
    private const int LevelPrefix = 1;
    private const string LevelName = "Scene2";

    public string PlayerName = "Underlord";
    public ServerInfo TargetServer;
    public event Action ServerListUpdated;

    private static NetworkManager _instance;
    private LanBroadcastService _broadcastService;
    private GameObject _gameController;

    public static NetworkManager Instance
    {
        get { return _instance; }
    }
    public List<ServerInfo> ServerList
    {
        get { return _broadcastService.ReceivedMessages.Select(rm => rm.Value.ServerInfo).ToList(); }
    }


    //public int listeningPort = 50005;
    //private string _ip = "172.21.66.8";

    void Awake()
    {
        if (_instance == null)
        {
            Init();
            return;
        }
        Debug.Log("Destorying multiple game manager");
        DestroyObject(gameObject);



    }

    private void Init()
    {
        _instance = this;
        _gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
        _broadcastService = gameObject.AddComponent<LanBroadcastService>();
        _broadcastService.ServerListUpdated += OnServerListUpdated;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(_gameController);
        Application.runInBackground = true;
    }

    public void StartServer()
    {
        Network.Disconnect();
        Network.InitializeServer(1, NetworkConfiguration.LAN_TCP_PORT, !Network.HavePublicAddress());
    }

    public void StartSearching()
    {
        _broadcastService.StartSearching();
    }

    public void StopBroadcastSession()
    {
        _broadcastService.StopSession();
    }

    public void ConnectToServer()
    {
        StopBroadcastSession();
        Network.Connect(TargetServer.IP, NetworkConfiguration.LAN_TCP_PORT);
    }

    private void OnServerListUpdated()
    {
        if (ServerListUpdated != null)
            ServerListUpdated();
    }

    void OnServerInitialized()
    {
        _gameController.networkView.RPC("SetGameState", RPCMode.AllBuffered, (int)GameController.GameState.Starting);
        _gameController.networkView.RPC("AddNetworkPlayerController", RPCMode.AllBuffered, (int)PlayerIDs.PlayerId.Player1,
            PlayerName, (int)Team.TeamIdentifier.Team1, Network.player);
        _broadcastService.StartAnnounceBroadCasting(PlayerName, 2);
    }

    private void OnPlayerConnected(NetworkPlayer player)
    {
        StopBroadcastSession();
        networkView.RPC("LoadLevel", RPCMode.AllBuffered);
    }


    //// Update is called once per frame
    //void Update()
    //{
    //    if (Network.isServer)
    //    {
    //        if (oneVSOne && Network.connections.Length == 1 && !gameStartet)
    //        {
    //            gameStartet = true;
    //            networkView.RPC("LoadLevel", RPCMode.AllBuffered, levelName, levelIndex);
    //        }
    //        else if (twoVSTwo && Network.connections.Length == 3 && !gameStartet)
    //        {
    //            networkView.RPC("LoadLevel", RPCMode.AllBuffered, levelName, levelIndex);
    //            gameStartet = true;
    //        }
    //    }
    //}

    //private void OnGUI()
    //{
    //    //if (nameDefined && !oneVSOne && !twoVSTwo)
    //    //{
    //    if (!Network.isClient && !Network.isServer && !startServer)
    //    {
    //        if (GUI.Button(new Rect(Screen.width * 0.01f, Screen.height * 0.01f, Screen.width * 0.1f, Screen.height * 0.1f),
    //                       "Start Server"))
    //        {
    //            //startServer = true;
    //            StartServer();
    //        }

    //        if (GUI.Button(new Rect(Screen.width * 0.01f, Screen.height * 0.12f, Screen.width * 0.1f, Screen.height * 0.1f),
    //                       "Join Server"))
    //        {
    //            Debug.Log("Joining Server");
    //            Network.Connect(_ip, listeningPort);
    //        }
    //    }
    //}

    public void CloseConnection()
    {
        Network.Disconnect();
        StopBroadcastSession();
    }

    void OnConnectedToServer()
    {
        _gameController.networkView.RPC("AddNetworkPlayerController", RPCMode.AllBuffered, (int)PlayerIDs.PlayerId.Player2,
            PlayerName, (int)Team.TeamIdentifier.Team2, Network.player);
    }

    [RPC]
    void LoadLevel()
    {
        //There is no reason to send any more data over the network on the default channel,
        //because we are about to load the level, thus all those objects will get deleted anyway
        Network.SetSendingEnabled(0, false);

        //We need to stop receiving because first the level must be loaded first.
        //Once the level is loaded, rpc's and other state update attached to objects in the level are allowed to fire
        Network.isMessageQueueRunning = false;

        //All network views loaded from a level will get a prefix into their NetworkViewID.
        //This will prevent old updates from clients leaking into a newly created scene.
        Network.SetLevelPrefix(LevelPrefix);
        Application.LoadLevel(LevelName);

        //Allow receiving data again
        Network.isMessageQueueRunning = true;
        //Now the level has been loaded and we can start sending out data to clients
        Network.SetSendingEnabled(0, true);
    }

    public static bool isNetwork
    {
        get { return Network.isServer || Network.isClient; }
    }

}
