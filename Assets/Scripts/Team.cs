using UnityEngine;
using System.Collections;

public class Team : MonoBehaviour {

	public enum TeamIdentifier {Team1, Team2}
	public TeamIdentifier ID;

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
}
