using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class Valve : MonoBehaviour
{


	public class ValveOccupant
	{
		public int player;
		public int minionCount;
		public float productivity;
		public int team;
	}
	public enum ValveState { Closed, Opened, FullyOccupied, NotFullyOccupied, NotOccupied }

    public const float _openValve = 100.0f;
    public const int _maxMinionCount = 5;

    public Team _valveTeam;
    public float _state = 100.0f;

	public Team.TeamIdentifier _occupant;
    public List<ValveOccupant> _occupants;
    public float _productivity = 0.0f;
    public int _minionCount = 0;


    // testing
    public ValveState currentState;
    // *********************

    public int _localPlayerID = -1;
    public List<MinionAgent> _localMinions;
    public float _localProductivity = 0.0f;
    public int _localMinionCount;
    

	public float State 
	{
		get
		{
			return _state;
		}
	}

	// Use this for initialization
	void Awake ()
	{
        _valveTeam = gameObject.GetComponent<Team>();
	    _occupant = _valveTeam.ID;
	    _localMinions = new List<MinionAgent>();
		_occupants = new List<ValveOccupant>();

	}

    void FindLocalPlayerID()
    {
        NetworkPlayerController netPlayer = GameObject.FindGameObjectWithTag(Tags.localPlayerController).GetComponent<LocalPlayerController>().networkPlayerController;
        if(netPlayer != null)
            _localPlayerID = netPlayer.playerID;
    }
	
	// Update is called once per frame
	void Update ()
	{
        for (int i = _localMinions.Count - 1; i > 0; i--)
        {
            if (_localMinions[i] == null)
            {
                RemoveMinion(_localMinions[i]);
                continue;
            }
                
            if (!_localMinions[i].gameObject.GetComponent<Health>().IsAlive())
                RemoveMinion(_localMinions[i]);
        }

        if(_localPlayerID == -1)
            FindLocalPlayerID();
	    currentState = GetValveState();

		if (networkView.isMine)
		{
			if (!_valveTeam.isEnemy(_occupant))
				_state += _productivity * Time.deltaTime;
            else if (_valveTeam.isEnemy(_occupant))
                _state -= _productivity * Time.deltaTime;
            Mathf.Clamp(_state, 0f, _openValve);
		}
        float tempProductivity = _localMinions.Sum(minion => minion.productivity); //sum of all localminions productivities
        if (tempProductivity != _localProductivity)
		{
            _localProductivity = tempProductivity;
			if (networkView.isMine)
			{
				SubmitLocalProductivity(_localProductivity, _localPlayerID);
			}
			else
			{
                networkView.RPC("SubmitLocalProductivity", networkView.owner, _localProductivity, _localPlayerID);
			}
		}
        if (_localMinionCount != _localMinions.Count)
        {
            _localMinionCount = _localMinions.Count;
            if (networkView.isMine)
            {
                SubmitLocalMinionCount(_localMinionCount, _localProductivity, _localPlayerID, (int)_occupant);
            }
            else
            {
                networkView.RPC("SubmitLocalMinionCount", networkView.owner, _localMinionCount, _localProductivity, _localPlayerID, (int)_occupant);
            }
        }


	}

    public bool stateComplete(MinionAgent minion)
    {
        return ((DoesValveBelongTo(minion) && GetValveState() == ValveState.Opened)
                || (!DoesValveBelongTo(minion) && GetValveState() == ValveState.Closed));
    }

    public bool isAvailable(MinionAgent minion)
    {
        return !(stateComplete(minion)
            || (_localMinions.Any(minionAgent => minionAgent == minion))
            || GetValveState() == ValveState.FullyOccupied
            || (_valveTeam.isEnemy(_occupant) && GetValveState() == ValveState.NotFullyOccupied)) ;
    }

	public void AddMinion(MinionAgent minion)
	{

        if (_localMinions.Any(minionAgent => minionAgent == minion)) return;
		_localMinions.Add(minion);
		_localProductivity = _localMinions.Sum(mini => mini.productivity);
	    _localMinionCount = _localMinions.Count;
        if (networkView.isMine)
        {
            SubmitLocalMinionCount(_localMinionCount, _localProductivity, _localPlayerID, (int)minion.GetComponent<Team>().ID);
        }
        else
        {
            networkView.RPC("SubmitLocalMinionCount", networkView.owner, _localMinionCount, _localProductivity, _localPlayerID, (int)minion.GetComponent<Team>().ID);
        }	
	}

	public bool RemoveMinion(MinionAgent minion)
	{
		for (int i = _localMinions.Count-1; i>=0;i--)
		{
			if (_localMinions[i].gameObject == minion.gameObject)
			{
                _localMinions.Remove(_localMinions[i]);
                _localProductivity = _localMinions.Sum(mini => mini.productivity);
                _localMinionCount = _localMinions.Count;
                if (networkView.isMine)
                {
                    SubmitLocalMinionCount(_localMinionCount, _localProductivity, _localPlayerID, (int)minion.GetComponent<Team>().ID);
                }
                else
                {
                    networkView.RPC("SubmitLocalMinionCount", networkView.owner, _localMinionCount, _localProductivity, _localPlayerID, (int)minion.GetComponent<Team>().ID);
                }
				return true;
			}
		}
		return false;
	}

	public ValveState GetValveState()
	{
        if (_state <= 0.0f)
        {
            return ValveState.Closed;
        }
        if (_state >= _openValve)
        {
            return ValveState.Opened;
        }
        
        if (_minionCount > 0)
		{
            if (_minionCount < _maxMinionCount)
			{
				return ValveState.NotFullyOccupied;
			}
			return ValveState.FullyOccupied;
		}
		return ValveState.NotOccupied;
	}

	private bool DoesValveBelongTo(MinionAgent minion)
	{
        return _valveTeam.isOwnTeam(minion.gameObject.GetComponent<Team>());
	}

	public virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		stream.Serialize(ref _state);
	}



    //Only called on Server
	[RPC]
	public void SubmitLocalMinionCount(int count, float productivity, int playerID, int team)
	{
        if(!networkView.isMine)
            return;
	    ValveOccupant submittedOccupant = null;
	    foreach (ValveOccupant valveOccupant in _occupants)
	    {
            if (valveOccupant.player == playerID)
            {
                submittedOccupant = valveOccupant;
            }
	    }
        if (submittedOccupant != null)
        {
            if (count == 0)
                _occupants.Remove(submittedOccupant);
            else
            {
                submittedOccupant.minionCount = count;
                submittedOccupant.productivity = productivity;
            }
        }
        else if(count != 0)
        {
            submittedOccupant = new ValveOccupant();
            submittedOccupant.minionCount = count;
            submittedOccupant.player = playerID;
            submittedOccupant.productivity = productivity;
            submittedOccupant.team = team;
            _occupants.Add(submittedOccupant);
            if (_occupants.Count == 1)
            {
                UpdateOccupant(team);
                networkView.RPC("UpdateOccupant", RPCMode.OthersBuffered, team);
            }
        }
        else
            return;

        _minionCount = _occupants.Sum(minion => minion.minionCount);
        _productivity = _occupants.Sum(minion => minion.productivity);
		networkView.RPC("UpdateMinionCount", RPCMode.OthersBuffered, _minionCount);
	}

	[RPC]
	public void SubmitLocalProductivity(float productivity, int playerID)
	{
        if(!networkView.isMine)
            return;
        ValveOccupant submittedOccupant = null;
        foreach (ValveOccupant valveOccupant in _occupants)
        {
            if (valveOccupant.player == playerID)
            {
                submittedOccupant = valveOccupant;
            }
        }
	    if (submittedOccupant != null)
	        submittedOccupant.productivity = productivity;
	}

	[RPC]
	public void UpdateMinionCount(int globalMinionCount)
	{
        _minionCount = globalMinionCount;
	}

	[RPC]
	public void UpdateOccupant(int team)
	{
		_occupant = (Team.TeamIdentifier)team;
	}
}
