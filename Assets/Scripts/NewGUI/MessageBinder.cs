using UnityEngine;
using System.Collections;

public class MessageBinder : MonoBehaviour
{
    private GameController gameController;
    
    public string message;
	
    // Use this for initialization
	void Awake ()
	{
	    gameController = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (message != gameController.Message)
	        message = gameController.Message;
	}
}
