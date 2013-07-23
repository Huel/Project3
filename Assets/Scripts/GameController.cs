using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	private float _startTime;

	public enum GameState {Starting, Running, Ended};

	public GameState state;

	private List<NetworkPlayerController> _allNetworkPlayerController = new List<NetworkPlayerController>();

	public List<NetworkPlayerController> AllNetworkPlayerController
	{
		get { return _allNetworkPlayerController; }
	}

	public float GameTime
	{
		get { return Time.time - _startTime; }
	}

	[RPC]
	void AddNetworkPlayerController(int playerID, string playerName, int Team, NetworkPlayer networkPlayer)
	{
		_allNetworkPlayerController.Add(new NetworkPlayerController(playerID, playerName, Team, networkPlayer));
	}

	[RPC]
	void SetGameState(int state)
	{
		this.state = (GameState)state;
		if (this.state == GameState.Running)
		{
			_startTime = Time.time;
		}
	}

	void Update()
	{
		
		if (state == GameState.Running)
		{
			Debug.Log(GameTime);
		}
		//if (_allNetworkPlayerController != null)
		//{
		//	foreach (NetworkPlayerController networkPlayerController in _allNetworkPlayerController)
		//	{
		//		Debug.Log(networkPlayerController.playerName);
		//	}
		//}
	}
}
