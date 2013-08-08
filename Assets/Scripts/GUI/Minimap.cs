using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minimap : MonoBehaviour
{
	public GameObject bluePointPrefab;
	public GameObject greenPointPrefab;

	private const float scale = 1.5f;
	private const float mapHight = 116;

	private GameObject heroTeamOnePoint;
	private GameObject heroTeamTwoPoint;

	public List<GameObject> players = new List<GameObject>();

	void Awake()
	{
		heroTeamOnePoint = (GameObject) Instantiate(bluePointPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		heroTeamOnePoint.transform.parent = gameObject.transform;
		//heroTeamOnePoint.transform.position = gameObject.transform.position;
		heroTeamOnePoint.transform.localEulerAngles = new Vector3(90, 0, 0);

		heroTeamTwoPoint = (GameObject)Instantiate(greenPointPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		heroTeamTwoPoint.transform.parent = gameObject.transform;
		//heroTeamTwoPoint.transform.position = gameObject.transform.position;
		heroTeamTwoPoint.transform.localEulerAngles = new Vector3(90, 0, 0);
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

	// Update is called once per frame
	void Update () 
	{
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
		AdjustMap();
	}
}
