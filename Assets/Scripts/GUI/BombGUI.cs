using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombGUI : MonoBehaviour
{
	public GameObject bombLane01;
	public GameObject bombLane02;
	public GameObject bombLane03;

	public List<GameObject> bombs = new List<GameObject>();

	private GUILane guiLane01;
	private GUILane guiLane02;
	private GUILane guiLane03;

	public List<GUILane> guiLanes = new List<GUILane>(); 

	private float team01X = 275;
	private float team02X = -35;

	// Use this for initialization
	void Awake ()
	{
		guiLane01 = transform.FindChild("lane01").GetComponent<GUILane>();
		guiLane02 = transform.FindChild("lane02").GetComponent<GUILane>();
		guiLane03 = transform.FindChild("lane03").GetComponent<GUILane>();
		bombs.Add(bombLane01);
		bombs.Add(bombLane02);
		bombs.Add(bombLane03);
		guiLanes.Add(guiLane01);
		guiLanes.Add(guiLane02);
		guiLanes.Add(guiLane03);
	}
	
	// Update is called once per frame
	void Update ()
	{
		ShowBombDirektion();
	}

	private void ShowBombDirektion()
	{
		for (int i = 0; i < bombs.Count; i++)
		{
			ShowArrows(bombs[i], guiLanes[i]);
			SetPosition(bombs[i], guiLanes[i]);
		}
	}

	private void ShowArrows(GameObject bomb, GUILane gui)
	{
		switch (bomb.GetComponent<Bomb>().BombDirection())
		{
			case (int) Team.TeamIdentifier.Team1:
				{
					gui._arrowBlue.gameObject.SetActive(true);
					gui._arrowRed.gameObject.SetActive(false);
					break;
				}
			case (int) Team.TeamIdentifier.Team2:
				{
					gui._arrowBlue.gameObject.SetActive(false);
					gui._arrowRed.gameObject.SetActive(true);
					break;
				}
			case -1:
				{
					gui._arrowBlue.gameObject.SetActive(false);
					gui._arrowRed.gameObject.SetActive(false);
					break;
				}
		}
	}

	private void SetPosition(GameObject bomb, GUILane gui)
	{
		if (bomb.transform.position.x <= team01X && bomb.transform.position.x >= team02X)
		{
			float ratio = (bomb.transform.position.x - team02X) / (team01X - team02X);

			gui._bombLaneBlue.fillAmount = 1 - ratio;
			gui._bomb.transform.localPosition = new Vector3((ratio * -6.8f) + 3.4f, gui._bomb.transform.localPosition.y, gui._bomb.transform.localPosition.z);
		}
		else
		{
			gui._arrowBlue.gameObject.SetActive(false);
			gui._arrowRed.gameObject.SetActive(false);
			bombs.Remove(bomb);
			guiLanes.Remove(gui);
		}
	}
}
