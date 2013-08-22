using UnityEngine;
using System.Collections;

public class ValveMotion : MonoBehaviour 
{  
	private float lastState;
	
	// Use this for initialization
	void Start () 
	{
		lastState = transform.parent.GetComponent<Valve>().State;
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (lastState < transform.parent.GetComponent<Valve>().State)
			gameObject.GetComponent<NetworkAnimator>().PlayAnimation("ValveRotation", false);
        else if (lastState > transform.parent.GetComponent<Valve>().State)
			gameObject.GetComponent<NetworkAnimator>().PlayAnimation("ValveRotation", true);

        lastState = transform.parent.GetComponent<Valve>().State;
	}
}
