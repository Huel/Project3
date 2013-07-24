using UnityEngine;

public class Team : MonoBehaviour
{

    public enum TeamIdentifier { Team1, Team2 }
    public TeamIdentifier ID;


    [RPC]
    public void setID(int ID)
    {
        this.ID = (TeamIdentifier)ID;
    }

    public void setID(TeamIdentifier ID)
    {
        this.ID = ID;
    }


    /// <summary>
    ///     checks if the other is an enemy
    /// </summary>
    /// <param name="ownTeam"></param>
    /// <returns></returns>
    public bool isEnemy(Team ownTeam)
    {
        return (this.ID != ownTeam.ID);
    }
    /// <summary>
    ///     checks if the other is in the same team
    /// </summary>
    /// <param name="ownTeam"></param>
    /// <returns></returns>
    public bool isOwnTeam(Team ownTeam)
    {
        return (this.ID == ownTeam.ID);
    }

    void Awake()
    {
        if (networkView != null && networkView.isMine)
        {
            setID(GameObject.FindGameObjectWithTag(Tags.localPlayerController).GetComponent<Team>().ID);
            networkView.RPC("setID", RPCMode.OthersBuffered, (int)ID);
        }
    }
}
