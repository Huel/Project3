using UnityEngine;
using System.Collections;

public class NetworkPlayerController
{
	public int playerID;
	public string playerName;
	public int team;
	public NetworkPlayer networkPlayer;

	public NetworkPlayerController(int playerID, string playerName, int team, NetworkPlayer networkPlayer)
	{
		this.playerID = playerID;
		this.playerName = playerName;
		this.team = team;
		this.networkPlayer = networkPlayer;
	}
}