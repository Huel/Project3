using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class LocalPlayerController : MonoBehaviour
{
    public NetworkPlayerController networkPlayerController;

    public GameObject MyBase { get; private set; }

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
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag(Tags.player).Any(Player => Player.GetComponent<NetworkView>().isMine))
        {
            return;
        }
        Object hero = Resources.Load("hero01");
        Network.Instantiate(hero, MyBase.transform.position, MyBase.transform.rotation, 1);
    }
}
