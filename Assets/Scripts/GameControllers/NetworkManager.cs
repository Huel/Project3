using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public int listeningPort = 50005;
    private string _ip = "172.21.66.8";
    public bool oneVSOne = true;
    public bool twoVSTwo = false;
    public bool startServer = false;

    private int levelIndex = 1;
    private string levelName = "Scene2";

    private bool gameStartet = false;
    //private bool nameDefined = false;

    public string playerName = "Enter Your Name";
    private GameObject gameController;

    public void setIP(string ip)
    {
        _ip = ip;
    }

    void Awake()
    {           
        GetComponent<NetworkSettings>().Init();
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Destorying multiple game manager");
            DestroyObject(gameObject);
            return;
        }

        Application.runInBackground = true;

        gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
        DontDestroyOnLoad(gameController);

    }

    // Update is called once per frame
    void Update()
    {
        if (Network.isServer)
        {
            if (oneVSOne && Network.connections.Length == 1 && !gameStartet)
            {
                gameStartet = true;
                networkView.RPC("LoadLevel", RPCMode.AllBuffered, levelName, levelIndex);
            }
            else if (twoVSTwo && Network.connections.Length == 3 && !gameStartet)
            {
                networkView.RPC("LoadLevel", RPCMode.AllBuffered, levelName, levelIndex);
                gameStartet = true;
            }
        }
    }

    private void OnGUI()
    {
        //if (nameDefined && !oneVSOne && !twoVSTwo)
        //{
        if (!Network.isClient && !Network.isServer && !startServer)
        {
            if (GUI.Button(new Rect(Screen.width * 0.01f, Screen.height * 0.01f, Screen.width * 0.1f, Screen.height * 0.1f),
                           "Start Server"))
            {
                //startServer = true;
                StartServer();
            }

            if (GUI.Button(new Rect(Screen.width * 0.01f, Screen.height * 0.12f, Screen.width * 0.1f, Screen.height * 0.1f),
                           "Join Server"))
            {
                Debug.Log("Joining Server");
                Network.Connect(_ip, listeningPort);
            }
        }

        //if (startServer)
        //{
        //	if (GUI.Button(new Rect(Screen.width*0.01f, Screen.height*0.01f, Screen.width*0.1f, Screen.height*0.1f),
        //				   "One VS One"))
        //	{
        //		Debug.Log("One VS One");
        //		oneVSOne = true;
        //		Debug.Log("Starting Server");
        //		StartServer();
        //	}

        //	if (GUI.Button(new Rect(Screen.width*0.01f, Screen.height*0.12f, Screen.width*0.1f, Screen.height*0.1f),
        //				   "Two VS Two"))
        //	{
        //		Debug.Log("Two VS Two");
        //		twoVSTwo = true;
        //		Debug.Log("Starting Server");
        //		StartServer();
        //	}
        //}
        //}
        //else if(!nameDefined)
        //{
        //	playerName = GUI.TextField(new Rect(Screen.width*0.01f, Screen.height*0.01f, Screen.width*0.1f, Screen.height*0.1f),
        //				  playerName);

        //	if (GUI.Button(new Rect(Screen.width * 0.01f, Screen.height * 0.12f, Screen.width * 0.1f, Screen.height * 0.1f),
        //				   "Set Name"))
        //	{
        //		nameDefined = true;
        //	}

        //}
    }

    private void StartServer()
    {
        Network.InitializeServer(oneVSOne ? 1 : 3, listeningPort, !Network.HavePublicAddress());
    }

    void OnServerInitialized()
    {
        Debug.Log("Server initialized");
        gameController.networkView.RPC("SetGameState", RPCMode.AllBuffered, (int)GameController.GameState.Starting);
        gameController.networkView.RPC("AddNetworkPlayerController", RPCMode.AllBuffered, (int)PlayerIDs.PlayerId.Player1,
            playerName, (int)Team.TeamIdentifier.Team1, Network.player);
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected!");
        switch (Network.connections.Length)
        {
            case 1:
                {
                    gameController.networkView.RPC("AddNetworkPlayerController", RPCMode.AllBuffered, (int)PlayerIDs.PlayerId.Player2,
                        playerName, (int)Team.TeamIdentifier.Team2, Network.player);
                    break;
                }
            case 2:
                {
                    gameController.networkView.RPC("AddNetworkPlayerController", RPCMode.AllBuffered, (int)PlayerIDs.PlayerId.Player3,
                        playerName, (int)Team.TeamIdentifier.Team1, Network.player);
                    break;
                }
            case 3:
                {
                    gameController.networkView.RPC("AddNetworkPlayerController", RPCMode.AllBuffered, (int)PlayerIDs.PlayerId.Player4,
                        playerName, (int)Team.TeamIdentifier.Team2, Network.player);
                    break;
                }
        }
    }

    [RPC]
    void LoadLevel(string level, int levelPrefix)
    {
        //There is no reason to send any more data over the network on the default channel,
        //because we are about to load the level, thus all those objects will get deleted anyway
        Network.SetSendingEnabled(0, false);

        //We need to stop receiving because first the level must be loaded first.
        //Once the level is loaded, rpc's and other state update attached to objects in the level are allowed to fire
        Network.isMessageQueueRunning = false;

        //All network views loaded from a level will get a prefix into their NetworkViewID.
        //This will prevent old updates from clients leaking into a newly created scene.
        Network.SetLevelPrefix(levelPrefix);
        Application.LoadLevel(level);

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
