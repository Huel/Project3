using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minimap : MonoBehaviour
{
	public GameObject teamOnePointPrefab;
	public GameObject teamTwoPointPrefab;
	public GameObject heroTeamOneArrowPrefab;
	public GameObject heroTeamTwoArrowPrefab;
	public GameObject heroTeamOnePointPrefab;
	public GameObject heroTeamTwoPointPrefab;

	public GameObject mapArrowsLane01;
	public GameObject mapArrowsLane02;
	public GameObject mapArrowsLane03;

	public bool minionManagerActive = false;

	private const float _mapHeight = 116;

	private GameObject heroTeamOneArrow;
	private GameObject heroTeamOnePoint;
	private GameObject heroTeamTwoArrow;
	private GameObject heroTeamTwoPoint;

	public List<GameObject> players = new List<GameObject>();
	public List<GameObject> minionTeamOne = new List<GameObject>();
	public List<GameObject> minionTeamTwo = new List<GameObject>();

	void Awake()
	{
		heroTeamOneArrow = (GameObject)Instantiate(heroTeamOneArrowPrefab, new Vector3(2000, 0, 0), Quaternion.identity);
		heroTeamOneArrow.transform.parent = gameObject.transform;
		heroTeamOnePoint = (GameObject)Instantiate(heroTeamOnePointPrefab, new Vector3(2000, 0, 0), Quaternion.identity);
		heroTeamOnePoint.transform.localEulerAngles = new Vector3(90, 0, 0);
		heroTeamOnePoint.transform.parent = gameObject.transform;

		heroTeamTwoArrow = (GameObject)Instantiate(heroTeamTwoArrowPrefab, new Vector3(2000, 0, 0), Quaternion.identity);
		heroTeamTwoArrow.transform.parent = gameObject.transform;
		heroTeamTwoPoint = (GameObject)Instantiate(heroTeamTwoPointPrefab, new Vector3(2000, 0, 0), Quaternion.identity);
		heroTeamTwoPoint.transform.localEulerAngles = new Vector3(90, 0, 0);
		heroTeamTwoPoint.transform.parent = gameObject.transform;
	}

	private GameObject CreatMinionPoint(GameObject prefab)
	{
		GameObject point;
		point = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
		point.transform.localEulerAngles = new Vector3(90, 0, 0);
		point.transform.parent = gameObject.transform;
		return point;
	}

	private void AdjustMap()
	{
		foreach (GameObject player in players)
		{
			if (player.networkView.isMine)
			{
				#region MiniMap scrolling

				if (!minionManagerActive)
				{
					if (player.transform.position.x < 222 && player.transform.position.x > 70)
					{
						float xPosition = player.transform.position.x;
						gameObject.transform.position = new Vector3(xPosition, 106, 16);
						float ratio = 1 - (xPosition - 70)/(222 - 70);
						mapArrowsLane01.transform.localPosition = new Vector3((ratio*140) - 70, 0, -90);
						mapArrowsLane02.transform.localPosition = new Vector3((ratio*140) - 70, 0, 0);
						mapArrowsLane03.transform.localPosition = new Vector3((ratio*140) - 70, 0, 90);
					}

					if (player.transform.position.x > 222)
					{
						gameObject.transform.position = new Vector3(222, 106, 16);
						mapArrowsLane01.transform.localPosition = new Vector3(-70, 0, -90);
						mapArrowsLane02.transform.localPosition = new Vector3(-70, 0, 0);
						mapArrowsLane03.transform.localPosition = new Vector3(-70, 0, 90);
					}

					if (player.transform.position.x < 70)
					{
						gameObject.transform.position = new Vector3(70, 106, 16);
						mapArrowsLane01.transform.localPosition = new Vector3(70, 0, -90);
						mapArrowsLane02.transform.localPosition = new Vector3(70, 0, 0);
						mapArrowsLane03.transform.localPosition = new Vector3(70, 0, 90);
					}
				}
				else
				{
					//gameObject.transform.position = new Vector3(147, 106, 16);
					mapArrowsLane01.transform.localPosition = new Vector3(0, 0, -90);
					mapArrowsLane02.transform.localPosition = new Vector3(0, 0, 0);
					mapArrowsLane03.transform.localPosition = new Vector3(0, 0, 90);
				}

				#endregion

				if (player.GetComponent<Team>().ID == Team.TeamIdentifier.Team1)
				{
					heroTeamOneArrow.transform.position = new Vector3(-(player.transform.position.x - gameObject.transform.position.x - 110),
						_mapHeight, player.transform.position.z);
					heroTeamOneArrow.transform.localEulerAngles = new Vector3(90, -player.transform.localEulerAngles.y, 0);
					if (!minionManagerActive) GameObject.FindGameObjectWithTag(Tags.cameraMinimap).transform.localEulerAngles = new Vector3(270, 90, 0);
				}
				else
				{
					heroTeamTwoArrow.transform.position = new Vector3(-(player.transform.position.x - gameObject.transform.position.x - 110),
						_mapHeight, player.transform.position.z);
					heroTeamTwoArrow.transform.localEulerAngles = new Vector3(90, -player.transform.localEulerAngles.y, 0);
					if (!minionManagerActive) GameObject.FindGameObjectWithTag(Tags.cameraMinimap).transform.localEulerAngles = new Vector3(270, 270, 0);
				}
			}
			else
			{
				if (player.GetComponent<Team>().ID == Team.TeamIdentifier.Team1)
				{
					heroTeamOnePoint.transform.position = new Vector3(-(player.transform.position.x - gameObject.transform.position.x - 110),
						_mapHeight, player.transform.position.z);

				}
				else
				{
					heroTeamTwoPoint.transform.position = new Vector3(-(player.transform.position.x - gameObject.transform.position.x - 110),
						_mapHeight, player.transform.position.z);
				}
			}
		}
	}

	void SearchPlayer()
	{
		#region fill List of Players
		if (players.Count < 2)
		{
			foreach (GameObject player in GameObject.FindGameObjectsWithTag(Tags.player))
			{
				if (!players.Contains(player) && player.GetComponent<Health>().IsAlive())
				{
					players.Add(player);
				}
			}
		}
		#endregion

		#region remove dead plyers
		foreach (GameObject player in GameObject.FindGameObjectsWithTag(Tags.player))
		{
			if (!player.GetComponent<Health>().IsAlive())
			{
				players.Remove(player);
			}
		}
		#endregion
	}

	private void PlaceMinions()
	{
		minionTeamOne = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tags.blueDot));
		minionTeamTwo = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tags.redDot));

		foreach (GameObject dot in minionTeamOne)
		{
			dot.transform.localScale = new Vector3(9.2f, 9.2f, 0);
		}
		
		foreach (GameObject dot in minionTeamTwo)
		{
			dot.transform.localScale = new Vector3(9.2f, 9.2f, 0);
		}

		foreach (GameObject minion in GameObject.FindGameObjectsWithTag(Tags.minion))
		{
			if (minion.GetComponent<Health>().IsAlive())
			{
				if (minion.GetComponent<Team>().ID == Team.TeamIdentifier.Team1)
				{
					if (minionTeamOne.Count > 0)
					{
						minionTeamOne[0].transform.position =
							new Vector3(-(minion.transform.position.x - gameObject.transform.position.x - 100),
							            _mapHeight, minion.transform.position.z);
						minionTeamOne.RemoveAt(0);
					}
					else
					{
						CreatMinionPoint(teamOnePointPrefab).transform.position =
							new Vector3(-(minion.transform.position.x - gameObject.transform.position.x - 100),
							            _mapHeight, minion.transform.position.z);
					}
				}
				else
				{
					if (minionTeamTwo.Count > 0)
					{
						minionTeamTwo[0].transform.position =
							new Vector3(-(minion.transform.position.x - gameObject.transform.position.x - 100),
							            _mapHeight, minion.transform.position.z);
						minionTeamTwo.RemoveAt(0);
					}
					else
					{
						CreatMinionPoint(teamTwoPointPrefab).transform.position =
							new Vector3(-(minion.transform.position.x - gameObject.transform.position.x - 100),
							            _mapHeight, minion.transform.position.z);
					}
				}
			}
		}

		foreach (GameObject bluePoint in minionTeamOne)
		{
			bluePoint.transform.position = new Vector3(500, 0, 0);
		}

		foreach (GameObject greenPoint in minionTeamTwo)
		{
			greenPoint.transform.position = new Vector3(500, 0, 0);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		SearchPlayer();
		PlaceMinions();
		AdjustMap();
	}
}
