using UnityEngine;
using System.Collections;

public class ValveMotion : MonoBehaviour
{
    public GameObject display;

    public float currentRotation;
    public float lastRotation;

    public float rotFactor;
    private float rotPerMinion = 90;

    void Awake()
    {
        rotFactor = GetComponent<WorkAnimation>().GetCompleteTime()*9;
        currentRotation = lastRotation = display.transform.localEulerAngles.z;
    }

	// Update is called once per frame
	void Update ()
	{
        if (currentRotation <= lastRotation + rotPerMinion 
            && GetComponent<WorkAnimation>().EnqueueTimer() <= GetComponent<WorkAnimation>().TimeDistance())
	    {
            currentRotation += -GetComponent<Valve>().GetRotationDirection()*rotFactor*Time.deltaTime;
	    }

	    display.transform.localEulerAngles = new Vector3(0, 0, currentRotation);
        networkView.RPC("CheckRotation", RPCMode.Others, currentRotation);
	}

    public void Motion()
    {
        if (currentRotation >= lastRotation + rotPerMinion)
        {
            lastRotation += -GetComponent<Valve>().GetRotationDirection()*rotPerMinion;
            currentRotation = lastRotation;
        }
    }

    [RPC]
    public void CheckRotation(float rotZ)
    {
        if (GetComponent<Valve>().ValveState != ValveStates.FullyOccupied 
            && GetComponent<Valve>().ValveState != ValveStates.NotFullyOccupied) 

            return;
        foreach (GameObject minion in GetComponent<WorkAnimation>().minions)
            if (minion != null) return;

        display.transform.localEulerAngles = new Vector3(0, 0, rotZ);
    }
}
