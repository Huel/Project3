using UnityEngine;
using System.Collections;

public class networkviewTEST : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (networkView.isMine)
		{
			Debug.Log("ES IST MEIN");
		}
	}
	
}
