using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class LocalPlayerController : MonoBehaviour
{
	public NetworkPlayerController networkPlayerController;
	// Use this for initialization
	void Awake ()
	{
		List<NetworkPlayerController> allNetworkPlayerController =
			GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameController>().AllNetworkPlayerController;
		if (allNetworkPlayerController != null)
		{
			foreach (NetworkPlayerController networkPlayerController in allNetworkPlayerController)
			{
				if (networkPlayerController.networkPlayer == Network.player)
				{
					this.networkPlayerController = networkPlayerController;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
