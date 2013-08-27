using UnityEngine;
using System.Collections;

public class ValveMotion : MonoBehaviour
{
    public GameObject display;

    public float currentRotation;

    private float rotFactor;

    void Awake()
    {
        rotFactor = GetComponent<WorkAnimation>().getCompleteTime()*4.5f;
    }

	// Update is called once per frame
	void Update ()
	{
	    if (GetComponent<Valve>().GetRotationDirection() == -1)
            currentRotation += rotFactor*Time.deltaTime;
        else if (GetComponent<Valve>().GetRotationDirection() == 1)
            currentRotation -= rotFactor * Time.deltaTime;

	    display.transform.localEulerAngles = new Vector3(0, 0, currentRotation);
	}
}
