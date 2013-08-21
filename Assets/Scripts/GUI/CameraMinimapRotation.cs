using UnityEngine;
using System.Collections.Generic;

public class CameraMinimapRotation : MonoBehaviour
{
	private Vector2 angle  = new Vector2(0, 0);

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
		angle = new Vector2(gameObject.transform.localEulerAngles.y, gameObject.transform.localEulerAngles.z);

		foreach (GameObject component in mapComponents)
		{
			component.transform.localEulerAngles = new Vector3(90, angle.x, angle.y);
		}

		foreach (GameObject blueDot in GameObject.FindGameObjectsWithTag(Tags.blueDot))
		{
			blueDot.transform.localEulerAngles = new Vector3(90, angle.x, angle.y);
		}

		foreach (GameObject redDot in GameObject.FindGameObjectsWithTag(Tags.redDot))
		{
			redDot.transform.localEulerAngles = new Vector3(90, angle.x, angle.y);
		}

		foreach (GameObject heroDot in GameObject.FindGameObjectsWithTag(Tags.heroDot))
		{
			heroDot.transform.localEulerAngles = new Vector3(90, angle.x, angle.y);
		}
	}
}
