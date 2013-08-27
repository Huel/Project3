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
        rotFactor = GetComponent<WorkAnimation>().getCompleteTime()*9;
        currentRotation = lastRotation = display.transform.localEulerAngles.z;
    }

	// Update is called once per frame
	void Update ()
	{
        if (currentRotation <= lastRotation + rotPerMinion 
            && GetComponent<WorkAnimation>().EnqueueTimer() <= GetComponent<WorkAnimation>().TimeDistance())
	    {
	         if (GetComponent<Valve>().GetRotationDirection() == -1)
                currentRotation += rotFactor*Time.deltaTime;
            else if (GetComponent<Valve>().GetRotationDirection() == 1)
                currentRotation -= rotFactor * Time.deltaTime;
	    }

	    display.transform.localEulerAngles = new Vector3(0, 0, currentRotation);
	}

    public void Motion()
    {
        if (currentRotation >= lastRotation + rotPerMinion)
        {
            lastRotation += rotPerMinion;
            currentRotation = lastRotation;
        }
    }
}
