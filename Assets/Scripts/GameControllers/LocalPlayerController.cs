using UnityEngine;
using Object = UnityEngine.Object;

public class LocalPlayerController : MonoBehaviour
{
    public NetworkPlayerController networkPlayerController;
    private GameObject _myBase;

    [SerializeField]
    public int[] minionDeployment = new int[3];

    public float spawnJitter = 2;
    // Use this for initialization 

    void Awake()
    {
        if(Network.isServer)
            GameObject.FindGameObjectWithTag(Tags.gameController).networkView.RPC("SetGameState", RPCMode.AllBuffered, (int)GameController.GameState.Running);
        //For Testing
        minionDeployment[0] = 1;
        minionDeployment[1] = 1;
        minionDeployment[2] = 1;

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

    public void SpawnMinions()
    {
        for (int i = 0; i < minionDeployment.Length; i++)
        {
            for (int j = 0; j < minionDeployment[i]; j++)
            {
                Vector3 pos = _myBase.transform.position;
                pos.x += Random.Range(-spawnJitter, spawnJitter);
                pos.y += Random.Range(-spawnJitter, spawnJitter);
                Object minion = Resources.Load("minion");
                GameObject minionInstance = (GameObject)Network.Instantiate(minion, pos, _myBase.transform.rotation, 1);
                MinionAgent agent = minionInstance.GetComponent<MinionAgent>();
                agent.laneID = (MinionAgent.LaneIdentifier)i;
                agent.SetOrigin(_myBase.GetComponent<Target>());
                agent.SetDestination(_myBase.GetComponent<Base>().GetCheckPoint(agent.laneID));
            }

        }


    }

    public void setMinionDeployment(int identifier, int value)
    {
        minionDeployment[identifier - 1] = value;
    }

    public void setMinionDeployment(MinionAgent.LaneIdentifier lane, int count)
    {
        minionDeployment[(int)lane] = count;
    }

    public int GetMinionsOnLane(MinionAgent.LaneIdentifier lane)
    {
        return minionDeployment[(int)lane];
    }
}
