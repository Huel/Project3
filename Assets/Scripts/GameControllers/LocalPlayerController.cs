using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class LocalPlayerController : MonoBehaviour
{
    public GameObject base01;
    public GameObject base01_destroyed;
    public GameObject base02;
    public GameObject base02_destroyed;
    
    public NetworkPlayerController networkPlayerController;

    public GameObject MyBase { get; private set; }

    private GameController gameController;

    // Use this for initialization 

    void Awake()
    {
        if (Network.isServer)
            GameObject.FindGameObjectWithTag(Tags.gameController).networkView.RPC("SetGameState", RPCMode.AllBuffered, (int)GameController.GameState.Running);

        networkPlayerController = GameObject.FindGameObjectWithTag(Tags.gameController)
                  .GetComponent<GameController>()
                  .GetNetworkPlayerController(Network.player);
        if (networkPlayerController == null)
            Debug.LogError("Couldn't find NetworkPlayerController");

        GetComponent<Team>().ID = (Team.TeamIdentifier)networkPlayerController.team;

        foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag(Tags.baseArea).Where(spawnPoint => spawnPoint.GetComponent<Team>().IsOwnTeam(GetComponent<Team>())))
        {
            MyBase = spawnPoint;
        }

        gameController = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.pointsTeam1 > 0)
        {
            base02.SetActive(false);
            base02_destroyed.SetActive(true);
        }

        if (gameController.pointsTeam2 > 0)
        {
            base01.SetActive(false);
            base01_destroyed.SetActive(true);
        }
        
        if (GameObject.FindGameObjectsWithTag(Tags.player).Any(Player => Player.GetComponent<NetworkView>().isMine))
        {
            return;
        }
        Object hero = Resources.Load("hero01");
        Network.Instantiate(hero, MyBase.transform.position, MyBase.transform.rotation, 1);

        
    }
}
