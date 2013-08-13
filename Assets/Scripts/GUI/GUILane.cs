using UnityEngine;
using System.Collections;

public class GUILane : MonoBehaviour
{
	public UISprite _bomb;
	public UISprite _bombLaneBlue;
	public UISprite _bombLaneRed;
	public UISprite _arrowBlue;
	public UISprite _arrowRed;
	
	// Use this for initialization
	void Awake ()
	{
		_bomb = transform.FindChild("bombGUI").GetComponent<UISprite>();
		_bombLaneBlue = transform.FindChild("bombLaneBlue").GetComponent<UISprite>();
		_bombLaneRed = transform.FindChild("bombLaneRed").GetComponent<UISprite>();
		_arrowBlue = transform.FindChild("arrowBlue").GetComponent<UISprite>();
		_arrowRed = transform.FindChild("arrowRed").GetComponent<UISprite>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
