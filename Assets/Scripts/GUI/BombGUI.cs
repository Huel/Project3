using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombGUI : MonoBehaviour
{
	public GameObject bombLane01;
	public GameObject bombLane02;
	public GameObject bombLane03;

	public List<GameObject> bombs = new List<GameObject>();
	public List<UISprite> guiLanes = new List<UISprite>();
	public List<UISprite> guiBombs = new List<UISprite>();

	private float team01X = 323;
	private float team02X = -70;

	// Use this for initialization
	void Awake ()
	{
		guiLanes.Add(transform.FindChild("blue_progress_lane01").GetComponent<UISprite>());
		guiLanes.Add(transform.FindChild("blue_progress_lane02").GetComponent<UISprite>());
		guiLanes.Add(transform.FindChild("blue_progress_lane03").GetComponent<UISprite>());
		guiBombs.Add(transform.FindChild("bomb01").GetComponent<UISprite>());
		guiBombs.Add(transform.FindChild("bomb02").GetComponent<UISprite>());
		guiBombs.Add(transform.FindChild("bomb03").GetComponent<UISprite>());

		bombs.Add(bombLane01);
		bombs.Add(bombLane02);
		bombs.Add(bombLane03);
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
			//ShowArrows(bombs[i], guiLanes[i]);
			SetPosition(bombs[i], guiLanes[i], guiBombs[i]);
		}
	}

	private void ShowArrows(GameObject bomb, UISprite lane)
	{
		//switch (bomb.GetComponent<Bomb>().BombDirection())
		//{
		//	case (int) Team.TeamIdentifier.Team1:
		//		{
		//			gui._arrowBlue.gameObject.SetActive(true);
		//			gui._arrowRed.gameObject.SetActive(false);
		//			break;
		//		}
		//	case (int) Team.TeamIdentifier.Team2:
		//		{
		//			gui._arrowBlue.gameObject.SetActive(false);
		//			gui._arrowRed.gameObject.SetActive(true);
		//			break;
		//		}
		//	case -1:
		//		{
		//			gui._arrowBlue.gameObject.SetActive(false);
		//			gui._arrowRed.gameObject.SetActive(false);
		//			break;
		//		}
		//}
	}

	private void SetPosition(GameObject bomb, UISprite lane, UISprite guiBomb)
	{
		if (bomb.transform.position.x <= team01X && bomb.transform.position.x >= team02X)
		{
			float ratio = (bomb.transform.position.x - team02X) / (team01X - team02X);

			lane.fillAmount = ratio;
			guiBomb.transform.localPosition = new Vector3((bomb.transform.localPosition.x - 147), guiBomb.transform.localPosition.y, (bomb.transform.localPosition.z - 14));
		}
		else
		{
			//gui._arrowBlue.gameObject.SetActive(false);
			//gui._arrowRed.gameObject.SetActive(false);

			bombs.Remove(bomb);
			guiLanes.Remove(lane);
			guiBombs.Remove(guiBomb);
		}
	}
}
