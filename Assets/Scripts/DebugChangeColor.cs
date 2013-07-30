using UnityEngine;
using System.Collections;

public class DebugChangeColor : MonoBehaviour
{
	public enum Colors { Blue, Green, LightBlue, LightGreen, Red, Black };
	public Colors colors;

	private float passedTime = 0;
	private Color blue = new Color(0f, 0f, 1f, 1f);
	private Color lightBlue = new Color(0.2f, 0.2f, 1f, 1f);
	private Color green = new Color(0f, 1f, 0f, 1f);
	private Color lightGreen = new Color(0.4f, 1f, 0.4f, 1f);
	private Color red = new Color(1f, 0f, 0f, 1f);
	private Color black = new Color(0f, 0f, 0f, 1f);

	// Use this for initialization
	void Start () {
		SetColor((int)gameObject.GetComponent<Team>().ID);
	}

	[RPC]
	public void SetColor(int color)
	{
		colors = (Colors)color;
		switch ((Colors)color)
		{
			case Colors.Blue:
				{
					if (gameObject.GetComponent<Target>().type == TargetType.Minion)
						gameObject.transform.FindChild("mesh_minion").renderer.material.color = blue;

					if (gameObject.GetComponent<Target>().type == TargetType.Hero)
						gameObject.transform.FindChild("mesh_hero01").renderer.material.color = blue;
					
					if (networkView.isMine)
					{
						networkView.RPC("SetColor", RPCMode.OthersBuffered, color);
					}
					break;
				}
			case Colors.LightBlue:
				{
					if (gameObject.GetComponent<Target>().type == TargetType.Minion)
						gameObject.transform.FindChild("mesh_minion").renderer.material.color = lightBlue;

					if (gameObject.GetComponent<Target>().type == TargetType.Hero)
						gameObject.transform.FindChild("mesh_hero01").renderer.material.color = lightBlue;

					if (networkView.isMine)
					{
						networkView.RPC("SetColor", RPCMode.OthersBuffered, color);
					}
					break;
				}
			case Colors.Green:
				{
					if (gameObject.GetComponent<Target>().type == TargetType.Minion)
						gameObject.transform.FindChild("mesh_minion").renderer.material.color = green;

					if (gameObject.GetComponent<Target>().type == TargetType.Hero)
						gameObject.transform.FindChild("mesh_hero01").renderer.material.color = green;

					if (networkView.isMine)
					{
						networkView.RPC("SetColor", RPCMode.OthersBuffered, color);
					}
					break;
				}
			case Colors.LightGreen:
				{
					if (gameObject.GetComponent<Target>().type == TargetType.Minion)
						gameObject.transform.FindChild("mesh_minion").renderer.material.color = lightGreen;

					if (gameObject.GetComponent<Target>().type == TargetType.Hero)
						gameObject.transform.FindChild("mesh_hero01").renderer.material.color = lightGreen;

					if (networkView.isMine)
					{
						networkView.RPC("SetColor", RPCMode.OthersBuffered, color);
					}
					break;
				}
			case Colors.Red:
				{
					if (gameObject.GetComponent<Target>().type == TargetType.Minion)
						gameObject.transform.FindChild("mesh_minion").renderer.material.color = red;

					if (gameObject.GetComponent<Target>().type == TargetType.Hero)
						gameObject.transform.FindChild("mesh_hero01").renderer.material.color = red;

					//if (networkView.isMine)
					//{
					//	networkView.RPC("SetColor", RPCMode.OthersBuffered, color);
					//}
					break;
				}
			case Colors.Black:
				{
					if (gameObject.GetComponent<Target>().type == TargetType.Minion)
						gameObject.transform.FindChild("mesh_minion").renderer.material.color = black;

					if (gameObject.GetComponent<Target>().type == TargetType.Hero)
						gameObject.transform.FindChild("mesh_hero01").renderer.material.color = black;

					if (networkView.isMine)
					{
						networkView.RPC("SetColor", RPCMode.OthersBuffered, color);
					}
					break;
				}
		}
	}

	public void Attack()
	{
		if (gameObject.GetComponent<Team>().ID == Team.TeamIdentifier.Team1)
		{
			SetColor((int) Colors.LightBlue);
		}
		else
		{
			SetColor((int)Colors.LightGreen);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (colors == Colors.Red || colors == Colors.LightBlue || colors == Colors.LightGreen)
		{
			passedTime += Time.deltaTime;
		}

		if (passedTime >= 0.1f)
		{
			SetColor((int)gameObject.GetComponent<Team>().ID);
			passedTime = 0;
		}
	}
}
