using UnityEngine;
using System.Collections.Generic;

public class LaneArrows : MonoBehaviour 
{
	public List<GameObject> blueArrows = new List<GameObject>();
	public List<GameObject> redArrows = new List<GameObject>();


	// Use this for initialization
	void Awake () 
	{
		for (int i = 1; i <= gameObject.transform.childCount/2; i++)
		{
			blueArrows.Add(gameObject.transform.FindChild("blueArrow0" + i).gameObject);
		}
		for (int i = 1; i <= gameObject.transform.childCount / 2; i++)
		{
			redArrows.Add(gameObject.transform.FindChild("redArrow0" + i).gameObject);
		}
	}
}
