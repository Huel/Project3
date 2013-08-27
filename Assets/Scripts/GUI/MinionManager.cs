using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class MinionManager : MonoBehaviour
{

	private class MinionView //minion face
	{
		public Transform minionView;
		public Transform minionPointer;
		public bool selected;

		public MinionView(Transform view, Transform pointer)
		{
			minionView = view;
			minionPointer = pointer;
		}

		public void ShowPointer(Vector3 position)
		{
			minionPointer.gameObject.SetActive(true);
			minionPointer.localPosition = position;
		}

		public void HidePointer()
		{
			minionPointer.gameObject.SetActive(false);
		}
	}

	private class MinionFullInfo //white square
	{
		public Vector3 pos;
		public MinionView occupant;

		public MinionFullInfo(Vector3 value)
		{
			pos = value;
		}

		public void SetOccupant(MinionView minionView)
		{
			occupant = minionView;
		}
	}



	//////////////// PUBLIC ////////////////
	public enum MinionManagerState { Visible, Invisible };
	public float SpawnJitter { private get; set; }
	public float minionViewLerpSpeed = 10f;
	public float minionViewScaleSpeed = 4f;
	public GameObject guiMiniMap;
	public GameObject miniMap;
	////////////////////////////////////////


	//////////////// PRIVATE ///////////////
	private List<MinionView> minionViews; //links to the minions of the GUI
	private List<List<MinionFullInfo>> viewPositions; // pos the minions should be lerped to when moving as well as which minion is now at this pos
	private Transform backgroundView;
	private bool isVisible = true;
	private const float bigMinionX = 25f;
	private const float bigMinionY = 20.7f;
	private const float smallMinionX = 13f;
	private const float smallMinionY = 11f;
	private const float bigPointerX = 30f;
	private const float bigPointerY = 26f;
	private const float smallPointerX = 15.2f;
	private const float smallPointerY = 13.3f;
	private const float snapScalingAt = 0.003f;
	private Vector3 arrowPointerLane01 = new Vector3(-74, -118, 0);
	private Vector3 arrowPointerLane02 = new Vector3(-74, - 40, 0);
	private Vector3 arrowPointerLane03 = new Vector3(-74,   40, 0);
	private int selectedLane = 1;
	private bool selectionIsReset;
	private int selectedCount = 0;
	private bool initialized;
	private bool buttonPushed;
	private int minionsPerPlayer;
	private GameObject pointerArrow;
	private GameObject sqaudManager;
	////////////////////////////////////////

	public int MinionsPerPlayer
	{
		get { return minionsPerPlayer; }
		set { minionsPerPlayer = value; }
	}

	private static GameObject SpawnLocation
	{
		get
		{
			return
				GameObject.FindGameObjectWithTag(Tags.localPlayerController)
						  .GetComponent<LocalPlayerController>()
						  .MyBase;
		}
	}

	public MinionManagerState GetMinionManagerState()
	{
		return isVisible ? MinionManagerState.Visible : MinionManagerState.Invisible;
	}

	public void Init()
	{
		initialized = true;
		backgroundView = GameObject.Find("minion_manager_view").transform;  //background, need it to set it invisible
		minionViews = new List<MinionView>();

		minionsPerPlayer = Mathf.Clamp(minionsPerPlayer, 1, 9); //Players can't have less than one minion, and no more than 15 since there are only that many white spots to place them on
		for (int i = 0; i < 9; i++)
		{
			//Assemble List with all necessary minion and pointer representations
			minionViews.Add(new MinionView(GameObject.Find("Head0" + (i + 1)).transform, GameObject.Find("PointerBox0" + (i + 1)).transform));
		}
		//minionViews[0].selected = true;

		viewPositions = new List<List<MinionFullInfo>>();
		for (int i = 0; i < 3; i++)
		{
			viewPositions.Add(new List<MinionFullInfo>());
			for (int j = 0; j < 9; j++)
			{   //Assemble all the View Positions, i.e. all the spots the minions can be placed on top of
				viewPositions[i].Add(new MinionFullInfo(GameObject.Find("Lane_" + (i + 1)).transform.FindChild("Background0" + (j + 1).ToString()).localPosition));
			}
		}

		pointerArrow = GameObject.Find("PointerArrow").gameObject;
		sqaudManager = GameObject.FindGameObjectWithTag("SqaudManager");

		//from here on out basic Init is done, now placing initial minions evenly over the lanes

		int minionCounter = 0;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < Math.Truncate(minionsPerPlayer / 3F); j++)
			{
				viewPositions[i][j].SetOccupant(minionViews[minionCounter]);
				minionCounter++;
			}
		}

		if (minionsPerPlayer % 3 == 0)
		{
			SetInvisible();
			return;
		}

		if (minionsPerPlayer % 3 == 1)
		{
			int counter = 0;
			foreach (MinionFullInfo minionFullInfo in viewPositions[0])
			{
				if (minionFullInfo.occupant == null)
				{
					minionFullInfo.SetOccupant(minionViews[minionCounter]);
					minionCounter++;
					break;
				}
				counter++;
			}
		}
		else
		{
			int counter = 0;
			foreach (MinionFullInfo minionFullInfo in viewPositions[0])
			{
				if (minionFullInfo.occupant == null)
				{
					minionFullInfo.SetOccupant(minionViews[minionCounter]);
					minionCounter++;
					break;
				}
				counter++;
			}
			counter = 0;
			foreach (MinionFullInfo minionFullInfo in viewPositions[1])
			{
				if (minionFullInfo.occupant == null)
				{
					minionFullInfo.SetOccupant(minionViews[minionCounter]);
					minionCounter++;
					break;
				}
				counter++;
			}
		}
		SetInvisible();
	}

	void Update()
	{
		if (!initialized && GameObject.Find("minion_manager_view") != null)
		{
			isVisible = false;
			SetInvisible();
			for (int i = 0; i < 9; i++)
			{
				GameObject.Find("Head0" + (i + 1)).GetComponent<UISprite>().alpha = 0;
				GameObject.Find("PointerBox0" + (i + 1)).GetComponent<UISprite>().alpha = 0;
			}

			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					GameObject.Find("Lane_" + (i + 1)).transform.FindChild("Background0" + (j + 1).ToString()).gameObject.GetComponent<UISprite>().alpha = 0;
				}
			}
		}

		if (!initialized) return;

		UpdateMinionPositions(); //Goes through the lanes and fills all the holes by changing MinionFullInfo.occupant and MinionView.x and y
		UpdatePointers(); //Turns pointers visible on selected minions and invisible on not selected ones

		if (Input.GetButtonDown(InputTags.manager) || Input.GetKeyDown(KeyCode.A))
		{
			if (GetMinionManagerState() == MinionManagerState.Visible)
			{
				SetInvisible();
			}
			else
			{
				SetVisible();
			}
		}

		ResetIf(MinionManagerState.Invisible == GetMinionManagerState());

		if (GetMinionManagerState() == MinionManagerState.Visible)
		{
			selectionIsReset = false;
			HandleInput();
			SlideMinions(); //Uses Lerp to move and scale the visual component of minions to their new positions
		}
		else
		{
			TeleportMinions(); //Directly sets new scaling and position on visual component of minions
		}
	}

	private void UpdatePointers()
	{
		foreach (MinionView minionView in minionViews)
		{
			if (minionView.selected)
			{
				minionView.ShowPointer(minionView.minionView.localPosition);
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 9; j++)
					{
						if (viewPositions[i][j].occupant == minionView)
						{
							minionView.minionPointer.localScale = new Vector3(j == 0 ? bigPointerX : smallPointerX,
																			  j == 0 ? bigPointerY : smallPointerY,
							                                                  minionView.minionPointer.localScale.z);
						}
					}
				}
			}
			else
			{
				minionView.HidePointer();
			}
		}
	}

	private void UpdateMinionPositions()
	{
		for (int i = 0; i < 3; i++)
		{
			while (GetHole(i))
			{
				for (int j = 0; j < 8; j++)
				{
					if (viewPositions[i][j].occupant != null || viewPositions[i][j + 1].occupant == null) continue;
					viewPositions[i][j].occupant = viewPositions[i][j + 1].occupant;
					viewPositions[i][j + 1].occupant = null;
				}
			}
		}
	}

	private void HandleInput()
	{
		if (!buttonPushed)
		{
			if (Input.GetAxisRaw(InputTags.squadLane) != 0 || Input.GetAxisRaw(InputTags.squadSelection) != 0)
			{
				buttonPushed = true;
			}

			HandleDownInputIf(Input.GetAxisRaw(InputTags.squadLane) < -0.1);

			HandleUpInputIf(Input.GetAxisRaw(InputTags.squadLane) > 0.1);

			SelectOneMoreMinionIf(Input.GetAxisRaw(InputTags.squadSelection) > 0.1 && !(Input.GetAxisRaw(InputTags.squadLane) < -0.1 || Input.GetAxisRaw(InputTags.squadLane) > 0.1));

			DisableSelectionIf(Input.GetAxisRaw(InputTags.squadSelection) < -0.1 && !(Input.GetAxisRaw(InputTags.squadLane) < -0.1 || Input.GetAxisRaw(InputTags.squadLane) > 0.1));
		}
		else
		{
			if (Input.GetAxisRaw(InputTags.squadLane) == 0 && Input.GetAxisRaw(InputTags.squadSelection) == 0)
			{
				buttonPushed = false;
			}
		}
	}

	private void TeleportMinions()
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				if (viewPositions[i][j].occupant == null) break;
				viewPositions[i][j].occupant.minionView.localScale = GetFinalScaling(j, viewPositions[i][j].occupant);
				viewPositions[i][j].occupant.minionView.localPosition = viewPositions[i][j].pos;
			}
		}
	}

	private static Vector3 GetFinalScaling(int x, MinionView content)
	{
		return new Vector3(x == 0 ? bigMinionX : smallMinionX, x == 0 ? bigMinionY : smallMinionY, content.minionView.localScale.z);
	}

	private void SlideMinions()
	{
		foreach (MinionFullInfo minionFullInfo in viewPositions
			.SelectMany(minionFullInfos => minionFullInfos.TakeWhile(minionFullInfo => minionFullInfo.occupant != null))
			.Where(minionFullInfo => minionFullInfo.occupant.minionView.position != minionFullInfo.pos))
		{
			if (GetMagnitude(minionFullInfo.occupant.minionView.localPosition, minionFullInfo.pos) < 0.001f) //this if lerps positions
			{
				minionFullInfo.occupant.minionView.localPosition = minionFullInfo.pos;
			}
			else
			{
				minionFullInfo.occupant.minionView.localPosition = Vector3.Lerp(minionFullInfo.occupant.minionView.localPosition,
																		   minionFullInfo.pos,
																		   minionViewLerpSpeed * Time.deltaTime);
			}
		}


		foreach (List<MinionFullInfo> minionFullInfos in viewPositions)
		{
			foreach (MinionFullInfo minionFullInfo in minionFullInfos)
			{
				if (minionFullInfo.occupant == null) continue;

				Vector3 finalScaling = GetFinalScaling(minionFullInfos.IndexOf(minionFullInfo), minionFullInfo.occupant);

				if (minionFullInfo.occupant.minionView.localScale == finalScaling) continue;

				if (GetMagnitude(minionFullInfo.occupant.minionView.localScale, finalScaling) < snapScalingAt) //this if lerps scaling
				{
					minionFullInfo.occupant.minionView.localScale = finalScaling;
				}
				else
				{
					minionFullInfo.occupant.minionView.localScale = Vector3.Lerp(minionFullInfo.occupant.minionView.localScale,
																				 finalScaling,
																				 minionViewScaleSpeed * Time.deltaTime);
				}
			}
		}
	}

	private static float GetMagnitude(Vector3 from, Vector3 to)
	{
		return (to - from).magnitude;
	}

	private int GetEmptyIndex(int lane)
	{
		for (int index = 0; index < 9; index++)
		{
			if (viewPositions[lane - 1][index].occupant == null)
			{
				return index;
			}
		}
		Debug.Log("Looking for Empty Index but there is none. FIX IT. SelectMinion() in MinionManager.");
		return 0;
	}

	private List<MinionFullInfo> GetSelectedMinions(int lane)
	{
		List<MinionFullInfo> selectedMinions = new List<MinionFullInfo>();
		for (int index = 0; index < 9; index++)
		{
			if (viewPositions[lane - 1][index].occupant != null && viewPositions[lane - 1][index].occupant.selected)
			{
				selectedMinions.Add(viewPositions[lane - 1][index]);
			}
		}
		return selectedMinions;
	}

	private void DisableSelectionIf(bool value)
	{
		if (!value) return;

		selectedCount = 0;

		foreach (MinionFullInfo minionFullInfo in viewPositions.SelectMany(minionFullInfos => minionFullInfos.Where(minionFullInfo => minionFullInfo.occupant != null)))
		{
			minionFullInfo.occupant.selected = false;
		}

		//viewPositions[selectedLane - 1][0].occupant.selected = true;
	}

	private void SelectOneMoreMinionIf(bool value)
	{
		if (!value) return;
		if (viewPositions[selectedLane - 1][selectedCount].occupant == null) return;

		selectedCount++;
		//if (selectedCount > 1)
			viewPositions[selectedLane - 1][selectedCount - 1].occupant.selected = true;
	}

	private void HandleUpInputIf(bool value)
	{
		if (!value) return;
		if (selectedLane <= 1) return;
		if (selectedCount != 0)
		{
			selectedLane--;
			int previouslySelectedLane = selectedLane + 1;
			while (GetSelectedMinions(previouslySelectedLane).Count > 0)
			{
				viewPositions[selectedLane - 1][GetEmptyIndex(selectedLane)].SetOccupant(GetSelectedMinions(previouslySelectedLane)[0].occupant);
				GetSelectedMinions(previouslySelectedLane)[0].occupant = null;
			}
			SwitchArrow(selectedLane);
			return;
		}

		if (viewPositions[0][0].occupant == null && viewPositions[1][0].occupant == null) return;
		if (selectedLane == 2 && viewPositions[0][0].occupant == null) return;

		if (selectedLane == 3 && viewPositions[1][0].occupant == null)
		{
			selectedLane -= 2;
		}
		else
		{
			selectedLane--;
		}

		SwitchArrow(selectedLane);

		//viewPositions[(viewPositions[selectedLane][0].occupant != null ? selectedLane : (selectedLane + 1))][0].occupant.selected = false;
		//viewPositions[selectedLane - 1][0].occupant.selected = true;
	}

	private void HandleDownInputIf(bool value)
	{
		if (!value) return;
		if (selectedLane >= 3) return;
		if (selectedCount != 0)
		{
			selectedLane++;
			int previouslySelectedLane = selectedLane - 1;
			while (GetSelectedMinions(previouslySelectedLane).Count > 0)
			{
				viewPositions[selectedLane - 1][GetEmptyIndex(selectedLane)].SetOccupant(GetSelectedMinions(previouslySelectedLane)[0].occupant);
				GetSelectedMinions(previouslySelectedLane)[0].occupant = null;
			}
			SwitchArrow(selectedLane);
			return;
		}

		if (viewPositions[2][0].occupant == null && viewPositions[1][0].occupant == null) return;
		if (selectedLane == 2 && viewPositions[2][0].occupant == null) return;


		if (selectedLane == 1 && viewPositions[1][0].occupant == null)
		{
			selectedLane += 2;
		}
		else
		{
			selectedLane++;
		}

		SwitchArrow(selectedLane);

		//viewPositions[(viewPositions[selectedLane - 2][0].occupant != null ? selectedLane - 2 : (selectedLane - 3))][0].occupant.selected = false;
		//viewPositions[selectedLane - 1][0].occupant.selected = true;
	}

	private void SwitchArrow(int lane)
	{
		switch (lane)
		{
			case 1:
				{
					pointerArrow.transform.localPosition = arrowPointerLane01;
					break;
				}
			case 2:
				{
					pointerArrow.transform.localPosition = arrowPointerLane02;
					break;
				}
			case 3:
				{
					pointerArrow.transform.localPosition = arrowPointerLane03;
					break;
				}
		}
	}

	private void ResetIf(bool value)
	{
		if (!value) return;
		if (selectionIsReset) return;

		selectionIsReset = true;
		selectedLane = 1;

		foreach (MinionFullInfo minionFullInfo in viewPositions.SelectMany(minionFullInfos => minionFullInfos.Where(minionFullInfo => minionFullInfo.occupant != null)))
		{
			minionFullInfo.occupant.selected = false;
		}

		//if (viewPositions[0][0].occupant != null)
		//{
		//	viewPositions[0][0].occupant.selected = true;
		//}
		//else
		//{
		//	if (viewPositions[1][0].occupant != null)
		//	{
		//		viewPositions[1][0].occupant.selected = true;
		//	}
		//	else
		//	{
		//		viewPositions[2][0].occupant.selected = true;
		//	}
		//}
	}

	private bool GetHole(int lane)
	{
		for (int i = 0; i < 8; i++)
		{
			if (viewPositions[lane][i].occupant == null && viewPositions[lane][i + 1].occupant != null)
			{
				return true;
			}
		}
		return false;
	}

	public void SpawnMinions()
	{
		foreach (List<MinionFullInfo> minionFullInfos in viewPositions)
		{
			foreach (MinionFullInfo minionFullInfo in minionFullInfos)
			{
				if (minionFullInfo.occupant == null) continue;


				Vector3 pos = GameObject.FindGameObjectWithTag(Tags.localPlayerController).GetComponent<LocalPlayerController>().MyBase.transform.localPosition;
				pos.x += Random.Range(-SpawnJitter, SpawnJitter);
				pos.y += Random.Range(-SpawnJitter, SpawnJitter);
				Object minion = Resources.Load("minion");
				GameObject minionInstance = (GameObject)Network.Instantiate(minion, pos, SpawnLocation.transform.rotation, 1);
				MinionAgent agent = minionInstance.GetComponent<MinionAgent>();
				agent.SetOrigin(SpawnLocation.GetComponent<Target>());
				agent.SetDestination(SpawnLocation.GetComponent<Base>().GetCheckPoint((MinionAgent.LaneIdentifier)(viewPositions.IndexOf(minionFullInfos))));
			}
		}
	}

	public void SetInvisible()
	{
		if (!isVisible) return;
		isVisible = false;

		miniMap.GetComponent<Minimap>().minionManagerActive = isVisible;
		//guiMiniMap.transform.localPosition = new Vector3(-160, -160, 0);
		//guiMiniMap.transform.localScale = new Vector3(32.5f, 0, 32.5f);
		guiMiniMap.gameObject.SetActive(!isVisible);
		miniMap.transform.parent = null;
		miniMap.transform.localEulerAngles = new Vector3(0, 0, 0);
		miniMap.transform.localScale = new Vector3(1, 1, 1);

		sqaudManager.SetActive(true);

		foreach (MinionView local in minionViews)
		{
			local.minionView.GetComponent<UISprite>().alpha = 0;
			local.minionPointer.GetComponent<UISprite>().alpha = 0;
		}
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				GameObject.Find("Lane_" + (i + 1)).transform.FindChild("Background0" + (j + 1).ToString()).gameObject.GetComponent<UISprite>().alpha = 0;
			}
		}
	}

	public void SetVisible()
	{
		if (isVisible) return;
		isVisible = true;

		guiMiniMap.gameObject.SetActive(!isVisible);
		miniMap.GetComponent<Minimap>().minionManagerActive = isVisible;
		//guiMiniMap.transform.localPosition = new Vector3(-1200, -500, 0);
		//guiMiniMap.transform.localScale = new Vector3(32.5f, 0, 32.5f);
		GameObject.FindGameObjectWithTag(Tags.cameraMinimap).transform.localEulerAngles = new Vector3(270, 180, 0);
		miniMap.transform.parent = guiMiniMap.transform.parent;
		miniMap.transform.localPosition = new Vector3(-1000f, -458f, 249f);
		miniMap.transform.localEulerAngles = new Vector3(90, 0, 0);
		miniMap.transform.localScale = new Vector3(3.8f, 1, 3.8f);

		sqaudManager.SetActive(false);

		foreach (MinionView local in minionViews)
		{
			local.minionView.GetComponent<UISprite>().alpha = 1;
			local.minionPointer.GetComponent<UISprite>().alpha = 1;
		}
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				GameObject.Find("Lane_" + (i + 1)).transform.FindChild("Background0" + (j + 1).ToString()).gameObject.GetComponent<UISprite>().alpha = 1;
			}
		}
	}
}