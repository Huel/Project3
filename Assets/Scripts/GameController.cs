using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

	private float _gameTime;

	public enum GameState {Starting, Running, Ended};

	public GameState state;

	public List<NetworkPlayerController> networkPlayerController;

	public float GameTime
	{
		get { return _gameTime; }
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
