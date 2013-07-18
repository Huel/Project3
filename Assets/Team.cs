using UnityEngine;
using System.Collections;

public class Team : MonoBehaviour {

	public enum TeamIdentifier {Team1, Team2}
	public TeamIdentifier ID;

	public bool isEnemy(Team ownTeam)
	{
		return (this.ID != ownTeam.ID);
	}

	public bool isOwnTeam(Team ownTeam)
	{
		return (this.ID == ownTeam.ID);
	}
}
