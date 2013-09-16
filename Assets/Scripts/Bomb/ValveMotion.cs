using UnityEngine;
using System.Collections;

public class ValveMotion : MonoBehaviour
{
    public GameObject display;

    public float currentRotation;
    public float lastRotation;

    public float rotFactor;
    private float rotPerMinion = 180;

    private float rotSpeed = 9;

    void Awake()
    {
        rotFactor = GetComponent<WorkAnimation>().getCompleteTime()*rotSpeed;
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
	}

    public void Motion()
    {
        if (currentRotation >= lastRotation + rotPerMinion)
        {
            lastRotation += -GetComponent<Valve>().GetRotationDirection()*rotPerMinion;
            currentRotation = lastRotation;
        }
    }
}
