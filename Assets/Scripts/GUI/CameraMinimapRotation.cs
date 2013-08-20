using UnityEngine;
using System.Collections.Generic;

public class CameraMinimapRotation : MonoBehaviour
{

	private List<GameObject> mapComponents = new List<GameObject>();
	
	private void Awake()
	{
		mapComponents.Add(GameObject.FindGameObjectWithTag(Tags.map).transform.FindChild("bomb01").gameObject);
		mapComponents.Add(GameObject.FindGameObjectWithTag(Tags.map).transform.FindChild("bomb02").gameObject);
		mapComponents.Add(GameObject.FindGameObjectWithTag(Tags.map).transform.FindChild("bomb03").gameObject);
	}

	// Update is called once per frame
	void Update () 
	{
		foreach (GameObject component in mapComponents)
		{
			component.transform.localEulerAngles = new Vector3(90, gameObject.transform.localEulerAngles.y, gameObject.transform.localEulerAngles.z);
		}
	}
}
