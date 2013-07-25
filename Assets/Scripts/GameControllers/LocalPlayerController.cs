using UnityEngine;

public class LocalPlayerController : MonoBehaviour
{
    public NetworkPlayerController networkPlayerController;
    private GameObject _myBase;
    // Use this for initialization
    void Awake()
    {
        networkPlayerController = GameObject.FindGameObjectWithTag(Tags.gameController)
                  .GetComponent<GameController>()
                  .GetNetworkPlayerController(Network.player);
        if (networkPlayerController == null)
            Debug.LogError("Couldn't find NetworkPlayerController");

        GetComponent<Team>().ID = (Team.TeamIdentifier)networkPlayerController.team;

        foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag(Tags.baseArea))
        {
            if (spawnPoint.GetComponent<Team>().isOwnTeam(GetComponent<Team>()))
            {
                _myBase = spawnPoint;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isDead = true;
        foreach (GameObject Player in GameObject.FindGameObjectsWithTag(Tags.player))
        {
            if (Player.GetComponent<NetworkView>().isMine)
            {
                isDead = false;
            }
        }

        if (isDead)
        {
            Object hero = Resources.Load("hero01");
            Network.Instantiate(hero, _myBase.transform.position, _myBase.transform.rotation, 1);
        }
    }
}
