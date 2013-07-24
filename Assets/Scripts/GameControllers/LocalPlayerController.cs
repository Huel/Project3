using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerController : MonoBehaviour
{
    public NetworkPlayerController networkPlayerController;
    private GameObject _myBase;
    // Use this for initialization
    void Awake()
    {
        List<NetworkPlayerController> allNetworkPlayerController =
            GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameController>().AllNetworkPlayerController;
        if (allNetworkPlayerController != null)
        {
            foreach (NetworkPlayerController nPController in allNetworkPlayerController)
            {
                if (nPController.networkPlayer == Network.player)
                {
                    this.networkPlayerController = nPController;
                }
            }
        }

        GetComponent<Team>().ID = (Team.TeamIdentifier)networkPlayerController.team;

        foreach (GameObject Base in GameObject.FindGameObjectsWithTag(Tags.baseArea))
        {
            if (Base.GetComponent<Team>().isOwnTeam(GetComponent<Team>()))
            {
                _myBase = Base;
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
            GameObject heroInstance = (GameObject)Network.Instantiate(hero, _myBase.transform.position, _myBase.transform.rotation, 1);
        }
    }
}
