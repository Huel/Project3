using UnityEngine;
using System.Collections;

public class IsBackZoomAllowed : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8) transform.parent.GetComponent<CameraClippingCorrection>().LeavingZoomAllowed = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8) transform.parent.GetComponent<CameraClippingCorrection>().LeavingZoomAllowed = true;
    }
}
