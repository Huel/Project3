using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class valve : MonoBehaviour
{

	public enum ValveState { Closed, Opened, FullyOccupied, NotFullyOccupied, NotOccupied }
	public Team.TeamIdentifier occupant;
	public float productivity = 0.0f;

	private float _state = 0.0f;
	private int _minionCount = 0;
	private List<MinionAgent> _localMinions;
    private List<ValveOccupant> _occupants;
    private float _localProductivity = 0.0f;
    private float _localProductivitySave = 0.0f;
    private float decay = 0.0f;

	public float _openValve = 100.0f;
	public int _maxMinionCount = 5;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
        _localProductivitySave = _localMinions.Sum(mini => mini.productivity);
        if (_localProductivitySave != _localProductivity)
        {
            _localProductivity = _localProductivitySave;
            if (!networkView.isMine)
            {
                networkView.RPC("SubmitLocalProductivity", RPCMode.Server, _localProductivity, GetComponent<LocalPlayerController>().networkPlayerController.playerID);
            }
        }
	}

	public bool AddMinion(MinionAgent minion)
	{
		if (DoesValveBelongTo(minion) && GetValveState() == ValveState.Opened) //being occupied by team x but already fully opened, then minions from team x may not use it
		{
			return false;
		}
		if (!DoesValveBelongTo(minion) && _occupants.Count > 0) //valve occupied by enemy team, and at least one enemy is at valve, minion may not use it
		{
			return false;
		}
		if (GetValveState() == ValveState.FullyOccupied)
		{
			return false;
		}

		_localMinions.Add(minion);
        if (!networkView.isMine)
        {
            //SubmitLocalMinionCount 
            float localProductivity = _localMinions.Sum(mini => mini.productivity);
            networkView.RPC("SubmitLocalMinionCount", RPCMode.Server, _localMinions.Count, localProductivity,
                            GetComponent<LocalPlayerController>().networkPlayerController.playerID,
                            (int) minion.GetComponent<Team>().ID);
        }
        else
        {
            //gehört mir bereits was nun
        }
		return true;
	}

	public bool RemoveMinion(MinionAgent minion)
	{
		foreach (MinionAgent localMinion in _localMinions)
		{
			if (localMinion.gameObject == minion.gameObject)
			{
				_localMinions.Remove(localMinion);
				return true;
			}
		}
		return false;
	}

	public ValveState GetValveState()
	{
		if (_occupants.Count > 0)
		{
			if (_occupants.Count < _maxMinionCount)
			{
				return ValveState.NotFullyOccupied;
			}
			return ValveState.FullyOccupied;
		}

		if (_state <= 0.0f)
		{
			return ValveState.Closed;
		}
		if (_state >= _openValve)
		{
			return ValveState.Opened;
		}
		return ValveState.NotOccupied;
	}

	private bool DoesValveBelongTo(MinionAgent minion)
	{
		return (int)occupant == (int)minion.GetComponent<Team>().ID;
	}

	public virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting && networkView.isMine)
		{
			stream.Serialize(ref _state);
		}
	}

	public struct ValveOccupant
	{
		public int player;
		public int minionCount;
		public float productivity;
	}


    [RPC]
    public void SubmitLocalMinionCount(int count, float productivity, int playerID, int team)
    {
        //depending on input change decay?
    }

    [RPC]
    public void SubmitLocalProductivity(float productivity, int playerID)
    {
        //depending on productivity change decay?
    }
}
