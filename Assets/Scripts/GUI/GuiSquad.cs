using UnityEngine;
using System.Collections.Generic;

public class GuiSquad : MonoBehaviour {
	private GameObject _player;

	public List<GameObject> sqaudIcons = new List<GameObject>();

	private bool FindIcons()
	{
		if (sqaudIcons.Count == 5)
		{
			return true;
		}
		else
		{
			for (int i = 1; i < 6; i++)
			{
				sqaudIcons.Add(gameObject.transform.FindChild("MinionHeadSquad0" + i).gameObject);
			}
			return false;
		}
	}

	private void SqaudView()
	{
		if (FindPlayer() && FindIcons())
		{
			int memberInSqaud = _player.GetComponent<Squad>().squadMembers.Count;

			for (int i = 0; i < memberInSqaud; i++)
			{
				sqaudIcons[i].SetActive(true);
			}

			for (int i = memberInSqaud; i < 5; i++)
			{
				sqaudIcons[i].SetActive(false);
			}
		}
	}

	private bool FindPlayer()
	{
		if (_player)
		{
			return true;
		}
		GameObject[] players = GameObject.FindGameObjectsWithTag(Tags.player);
		foreach (GameObject player in players)
		{
			if (player.networkView.isMine)
			{
				_player = player;
				return true;
			}
		}
		return false;
	}

	// Update is called once per frame
	void Update ()
	{
		SqaudView();
	}
}
