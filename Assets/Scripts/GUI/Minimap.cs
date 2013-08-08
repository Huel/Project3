using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minimap : MonoBehaviour
{
	public GameObject teamOnePointPrefab;
	public GameObject teamTwoPointPrefab;
	public GameObject heroTeamOnePointPrefab;
	public GameObject heroTeamTwoPointPrefab;

	private const float scale = 13.8f;
	private const float mapHight = 116;

	private GameObject heroTeamOnePoint;
	private GameObject heroTeamTwoPoint;

	public List<GameObject> players = new List<GameObject>();
	public List<GameObject> minionTeamOne = new List<GameObject>();
	public List<GameObject> minionTeamTwo = new List<GameObject>();

	void Awake()
	{
		heroTeamOnePoint = (GameObject)Instantiate(heroTeamOnePointPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		AdjustHeroPoint(heroTeamOnePoint);

		heroTeamTwoPoint = (GameObject)Instantiate(heroTeamTwoPointPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		AdjustHeroPoint(heroTeamTwoPoint);
	}

	private void AdjustHeroPoint(GameObject point)
	{
		point.transform.parent = gameObject.transform;
		point.transform.localScale = new Vector3(scale, scale);
		point.transform.localEulerAngles = new Vector3(90, 0, 0);
	}

	private GameObject CreatMinionPoint(GameObject prefab)
	{
		GameObject point;
		point = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
		point.transform.parent = gameObject.transform;
		point.transform.localEulerAngles = new Vector3(90, 0, 0);
		return point;
	}

	private void AdjustMap()
	{
		foreach (GameObject player in players)
		{
			if (player.networkView.isMine)
			{
				#region MiniMap scrolling

				if (player.transform.position.x < 222 && player.transform.position.x > 70)
				{
					gameObject.transform.position = new Vector3(player.transform.position.x, 106, 16);
				}
				
				if (player.transform.position.x > 222)
				{
					gameObject.transform.position = new Vector3(222, 106, 16);
				}

				if (player.transform.position.x < 70)
				{
					gameObject.transform.position = new Vector3(70, 106, 16);
				}
				#endregion

				if (player.GetComponent<Team>().ID == Team.TeamIdentifier.Team1)
				{
					heroTeamOnePoint.transform.position = new Vector3(-(player.transform.position.x - gameObject.transform.position.x - 100),
						mapHight, player.transform.position.z);
				}
				else
				{
					heroTeamTwoPoint.transform.position = new Vector3(-(player.transform.position.x - gameObject.transform.position.x - 100),
						mapHight, player.transform.position.z);
				}
			}
			else
			{
				if (player.GetComponent<Team>().ID == Team.TeamIdentifier.Team1)
				{
					heroTeamOnePoint.transform.position = new Vector3(-(player.transform.position.x - gameObject.transform.position.x - 100),
						mapHight, player.transform.position.z);
				}
				else
				{
					heroTeamTwoPoint.transform.position = new Vector3(-(player.transform.position.x - gameObject.transform.position.x - 100),
						mapHight, player.transform.position.z);
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
		minionTeamTwo = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tags.greenDot));

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
							            mapHight, minion.transform.position.z);
						minionTeamOne.RemoveAt(0);
					}
					else
					{
						CreatMinionPoint(teamOnePointPrefab).transform.position =
							new Vector3(-(minion.transform.position.x - gameObject.transform.position.x - 100),
							            mapHight, minion.transform.position.z);
					}
				}
				else
				{
					if (minionTeamTwo.Count > 0)
					{
						minionTeamTwo[0].transform.position =
							new Vector3(-(minion.transform.position.x - gameObject.transform.position.x - 100),
							            mapHight, minion.transform.position.z);
						minionTeamTwo.RemoveAt(0);
					}
					else
					{
						CreatMinionPoint(teamTwoPointPrefab).transform.position =
							new Vector3(-(minion.transform.position.x - gameObject.transform.position.x - 100),
							            mapHight, minion.transform.position.z);
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

	private 

	// Update is called once per frame
	void Update ()
	{
		SearchPlayer();
		PlaceMinions();
		AdjustMap();
	}
}
