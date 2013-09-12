using UnityEngine;
using System.Collections;

public class ValveDisplay : MonoBehaviour
{
    public GameObject display;

    public float currentRotation;
    
    private float minRotation = -90f;
    private float maxRotation = 90f;

    private float rotFactor;

	// Use this for initialization
	void Awake ()
	{
	    rotFactor = Mathf.Abs(maxRotation - minRotation)/100;
	}
	
	// Update is called once per frame
	void Update ()
	{
        currentRotation = GetComponent<Valve>().State*rotFactor + minRotation;
        display.transform.localEulerAngles = new Vector3(10, 90, currentRotation);  
    }
}
