using UnityEngine;

public class NetworkManager : MonoBehaviour
{

	public int listeningPort = 50005;
	public string ip = "172.21.66.8";

	public bool oneVsOne = true;

	//private bool spawnPlayer = false;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (oneVsOne && Network.connections.Length == 2)
		{

		}
	}

	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(Screen.width * 0.01f, Screen.height * 0.01f, Screen.width * 0.1f, Screen.height * 0.1f), "Start Server"))
			{
				Debug.Log("Starting Server");
				startServer();
			}

			if (GUI.Button(new Rect(Screen.width * 0.01f, Screen.height * 0.12f, Screen.width * 0.1f, Screen.height * 0.1f), "Join Server"))
			{
				Debug.Log("Joining Server");
				Network.Connect(ip, listeningPort);
			} 
		}
	}

	private void startServer()
	{
		Network.InitializeServer(4, listeningPort, !Network.HavePublicAddress());

	}

	void OnServerInitialized()
	{
		Debug.Log("Server initialized");
		GameObject.FindGameObjectWithTag(Tags.localPlayerControler).GetComponent<LocalPlayerControler>().teamID = 1;
	}

	void OnConnectedToServer()
	{
		Debug.Log("Connected!");
		if (oneVsOne)
		{
			GameObject.FindGameObjectWithTag(Tags.localPlayerControler).GetComponent<LocalPlayerControler>().teamID = 2;
		}
	}
}
