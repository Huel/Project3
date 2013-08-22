using UnityEngine;
using System.Collections;

public class ValveMotion : MonoBehaviour
{

    private int motionState = 0;
	// Update is called once per frame
	void Update ()
	{
        if (motionState != -1 && GetComponent<Valve>().GetRotationDirection() == -1)
        {
            GetComponent<NetworkAnimator>().PlayAnimation("ValveRotation", false);
            motionState = -1;
        }
        else if (motionState != 1 && GetComponent<Valve>().GetRotationDirection() == 1)
        {
            GetComponent<NetworkAnimator>().PlayAnimation("ValveRotation");
            motionState = 1;
        }
        else if (motionState != 0 && GetComponent<Valve>().GetRotationDirection() == 0)
        {
            GetComponent<NetworkAnimator>().StopAnimation();
            motionState = 0;
        }
	}
}
