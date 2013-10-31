using UnityEngine;

public class Team : MonoBehaviour
{

    public static Color[] teamColors;
    public enum TeamIdentifier { NoTeam, Team1, Team2 }
    public TeamIdentifier ID;


    [RPC]
    public void SetID(int ID)
    {
        this.ID = (TeamIdentifier)ID;
    }

    public void SetID(TeamIdentifier ID)
    {
        this.ID = ID;
    }


    /// <summary>
    ///     checks if the other is an enemy
    /// </summary>
    /// <param name="ownTeam"></param>
    /// <returns></returns>
    public bool IsEnemy(Team ownTeam)
    {
        return (this.ID != ownTeam.ID);
    }

    public bool IsEnemy(TeamIdentifier ownTeam)
    {
        return (this.ID != ownTeam);
    }

    public bool IsEnemy(int ownTeam)
    {
        return ((int)this.ID != ownTeam);
    }
    /// <summary>
    ///     checks if the other is in the same team
    /// </summary>
    /// <param name="ownTeam"></param>
    /// <returns></returns>
    public bool IsOwnTeam(TeamIdentifier ownTeam)
    {
        return (this.ID == ownTeam);
    }

    public bool IsOwnTeam(Team ownTeam)
    {
        return (this.ID == ownTeam.ID);
    }

    public bool IsOwnTeam(int ownTeam)
    {
        return ((int)this.ID == ownTeam);
    }

    public TeamIdentifier Other()
    {

        switch (ID)
        {
            case TeamIdentifier.Team1:
                return TeamIdentifier.Team2;
            case TeamIdentifier.Team2:
                return TeamIdentifier.Team1;
        }
        return TeamIdentifier.NoTeam;

    }

    void Awake()
    {
        if (NetworkManager.isNetwork && networkView != null && networkView.isMine && ID == TeamIdentifier.NoTeam)
        {
            SetID(GameObject.FindGameObjectWithTag(Tags.localPlayerController).GetComponent<Team>().ID);
            networkView.RPC("SetID", RPCMode.OthersBuffered, (int)ID);
        }
    }
}
