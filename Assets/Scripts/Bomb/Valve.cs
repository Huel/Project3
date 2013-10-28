using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public enum ValveStates { Closed, Opened, FullyOccupied, NotFullyOccupied, NotOccupied }

[RequireComponent(typeof(NetworkView))]
public class Valve : MonoBehaviour
{
    /*
     * The minions are instantiated on the PC of the player they belong to. Only there their logic will be executed. 
     * When they try to occupy a valve (happens in the minion agent component) it gets difficult because the 
     * logic of the valves is on the server. To have an interface for the minions the valve class is split in a 
     * local and a global part.
     * The valve class should also work in a 2 vs 2 mode.
     */

    //Internal class to store the different local data of the players on the server.
    public class ValveOccupant
    {
        //ID of the player
        public int player;
        //number of HIS minions working at the valve
        public int minionCount;
        //how fast are all the minions together
        public float productivity;
        //player's team ID
        public int team;
    }


    //The max value of the valve's state
    public const float _openValve = 100.0f;
    //maximum amount of the minions at the valve
    public const int _maxMinionCount = 5;


    //Global Data:
    //To which team belongs the valve
    private Team _teamComponent;
    private float _state;

    //Team of the Minions which are working at the valve
    private Team.TeamIdentifier _occupant;
    //List with all occupying players (for 2 vs. 2 relevant because there could be minions of two players at the same time)
    private List<ValveOccupant> _occupants;
    //Productivity of all minions, stored only on the server
    private float _productivity = 0.0f;
    //Amount of all minions
    private int _minionCount = 0;


    //Local Data:
    private int _localPlayerID = -1;
    private int _localTeam = 0;
    //list of the working minions
    private List<MinionAgent> _localMinions;
    //complete productivity of the local minions
    private float _localProductivity = 0.0f;
    private int _localMinionCount;
    private AudioLibrary soundLibrary;
    private XmlDocument document;


    //Properties, read only:
    public float State
    {
        get { return _state; }
    }
    public Team.TeamIdentifier Occupant
    {
        get { return _occupant; }
    }
    public int MinionCount
    {
        get { return _minionCount; }
    }
    public float Productivity
    {
        get { return _productivity; }
    }
    public int LocalPlayerID
    {
        get { return _localPlayerID; }
    }
    public int LocalMinionCount
    {
        get { return _localMinionCount; }
    }
    public float LocalProductivity
    {
        get { return _localProductivity; }
    }
    public List<MinionAgent> LocalMinions
    {
        get { return _localMinions; }
    }


    public ValveStates ValveState
    {
        get
        {
            if (_state <= 0.0f)
            {
                return ValveStates.Closed;
            }
            if (_state >= _openValve)
            {
                return ValveStates.Opened;
            }

            //If not completely closed or opened it's good to know if there are minions on it and if other minions can join
            if (_minionCount > 0)
            {
                if (_minionCount < _maxMinionCount)
                {
                    return ValveStates.NotFullyOccupied;
                }
                return ValveStates.FullyOccupied;
            }
            return ValveStates.NotOccupied;
        }
    }

    void Awake()
    {
        soundLibrary = transform.FindChild("sound_Valve").GetComponent<AudioLibrary>();
        document = new XMLReader("Minion.xml").GetXML();
        //Valve should be opened when the game starts
        _state = _openValve;
        _teamComponent = GetComponent<Team>();

        _occupant = Team.TeamIdentifier.NoTeam;
        _occupants = new List<ValveOccupant>();

        _localMinions = new List<MinionAgent>();
    }

    void Update()
    {
        //Set localPlayer ID if not already done
        if (_localPlayerID == -1)
            FindLocalPlayerID();
        RemoveDeadMinions();
        CheckLocalMinionCount();
        CheckLocalProductivity();
        UseValve();
    }

    //Synchronize the float state of the valve through the network view
    public virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        stream.Serialize(ref _state);
    }

    private void FindLocalPlayerID()
    {
        NetworkPlayerController netPlayer = GameObject.FindGameObjectWithTag(Tags.localPlayerController).GetComponent<LocalPlayerController>().networkPlayerController;
        if (netPlayer != null)
        {
            _localPlayerID = netPlayer.playerID;
            _localTeam = netPlayer.team;
        }
    }

    private void RemoveDeadMinions()
    {
        //Remove all null-References and all dead Minions from list
        _localMinions.RemoveAll(minion => minion == null || !minion.GetComponent<Health>().IsAlive());
        //Update the WorkAnimation-Component
        GetComponent<WorkAnimation>().CheckMinions(_localMinions);
    }

    private void CheckLocalMinionCount()
    {
        //Check if the minion count is up to date 
        //Or if you aren't the current occupant but you have minions at the valve => Update local minion count
        if (_localMinionCount != _localMinions.Count || (_localTeam != (int)_occupant && _localMinionCount > 0))
        {
            _localMinionCount = _localMinions.Count;
            //If you are the server you have to call the method normally, else send an RPC
            if (networkView.isMine)
                SubmitLocalMinionCount(_localMinionCount, _localPlayerID, _localTeam);
            else
                networkView.RPC("SubmitLocalMinionCount", networkView.owner, _localMinionCount, _localPlayerID, _localTeam);
        }
    }

    private void CheckLocalProductivity()
    {
        //sum of all local minion's productivities
        float tempProductivity = _localMinions.Sum(minion => minion.productivity);
        //Check if it differs from old value
        if (tempProductivity != _localProductivity)
        {
            _localProductivity = tempProductivity;
            if (networkView.isMine)
                SubmitLocalProductivity(_localProductivity, _localPlayerID);
            else
                networkView.RPC("SubmitLocalProductivity", networkView.owner, _localProductivity, _localPlayerID);
        }
    }

    private void UseValve()
    {
        //Server only! And only use when minions are on it
        if (networkView.isMine && InUse())
        {
            //Check the rotation direction
            if (_teamComponent.IsOwnTeam(_occupant))
                _state = Mathf.Clamp(_state + (_productivity * Time.deltaTime), 0f, _openValve);
            else if (_teamComponent.IsEnemy(_occupant))
                _state = Mathf.Clamp(_state - (_productivity * Time.deltaTime), 0f, _openValve);
        }
    }

    public void AddMinion(MinionAgent minion)
    {
        //The minion shouldn't be already in the list
        if (_localMinions.Any(minionAgent => minionAgent == minion)) return;
        _localMinions.Add(minion);
        //Add the minion to the work animation
        GetComponent<WorkAnimation>().PrepareMinion(minion.gameObject);
    }

    public void RemoveMinion(MinionAgent minion)
    {
        //Remove the minion even if he is per accident twice in the list
        _localMinions.RemoveAll(minionAgent => minionAgent == minion);
        //Remove also from work animation component
        GetComponent<WorkAnimation>().RemoveMinion(minion.gameObject);
    }

    //Checks if minion is already at valve
    public bool IsUsingValve(MinionAgent minion)
    {
        return _localMinions.Any(minionAgent => minionAgent == minion);
    }

    //Checks if the minion doesn't have to work at the valve
    public bool IsCompleted(MinionAgent minion)
    {
        return ((ValveBelongsTo(minion) && ValveState == ValveStates.Opened)
                || (!ValveBelongsTo(minion) && ValveState == ValveStates.Closed));
    }

    //Proves if the minion can be added to the valve
    //Minions can only call this method on the PC of the player to who they belong
    public bool IsAvailable(MinionAgent minion)
    {
        //returns true if the work is not already completed, the minion isn't already using the valve
        //and if there is place on the valve and no enemy already occupying the valve
        return (!IsCompleted(minion) && !IsUsingValve(minion)
            && (ValveState != ValveStates.FullyOccupied ||
                ((int)_occupant != _localTeam && InUse())));
    }

    //Checks if the minion belongs to the same team of the valve
    private bool ValveBelongsTo(MinionAgent minion)
    {
        return _teamComponent.IsOwnTeam(_localTeam);
    }

    //If there are minions on the valve it's in use
    public bool InUse()
    {
        return _minionCount > 0;
    }


    //Rotation direction for valve animations
    public int GetRotationDirection()
    {
        if (!InUse())
            return 0;
        if (_teamComponent.IsOwnTeam(_occupant))
            return 1;
        return -1;
    }


    //Only called on Server, every valve lies on the server
    [RPC]
    public void SubmitLocalMinionCount(int count, int playerID, int team)
    {
        //Check if it's really the server
        if (!networkView.isMine)
            return;

        ValveOccupant submittedOccupant = null;
        foreach (ValveOccupant valveOccupant in _occupants)
        {
            //Check if the player information is already on the server
            if (valveOccupant.player == playerID)
            {
                submittedOccupant = valveOccupant;
            }
        }
        //in case of a match update it
        if (submittedOccupant != null)
        {
            //if the number of minions equals 0 than remove the occupant from the list
            if (count == 0)
                _occupants.Remove(submittedOccupant);
            else
            {
                //else update the minion count
                submittedOccupant.minionCount = count;
            }
        }
        else if (count != 0)
        {
            //if no matches but count higher than 0, create new occupant information for the player and add it to the list
            submittedOccupant = new ValveOccupant();
            submittedOccupant.minionCount = count;
            submittedOccupant.player = playerID;
            submittedOccupant.team = team;
            _occupants.Add(submittedOccupant);

            //Change the current occupant and the it per RPC to the clients
            UpdateOccupant(team);
            networkView.RPC("UpdateOccupant", RPCMode.OthersBuffered, team);

        }
        else
            return;

        //if an occupant has changed update also the minion count on server and clients
        _minionCount = _occupants.Sum(minion => minion.minionCount);
        //productivity is only necessary for the server because there is the "valve logic"
        _productivity = _occupants.Sum(minion => minion.productivity);
        networkView.RPC("UpdateMinionCount", RPCMode.OthersBuffered, _minionCount);
    }

    [RPC]
    public void SubmitLocalProductivity(float productivity, int playerID)
    {
        if (!networkView.isMine)
            return;

        ValveOccupant submittedOccupant = null;
        foreach (ValveOccupant valveOccupant in _occupants)
        {
            if (valveOccupant.player == playerID)
            {
                submittedOccupant = valveOccupant;
            }
        }
        //if no occupant found don't update the productivity
        if (submittedOccupant != null)
            submittedOccupant.productivity = productivity;

    }

    //RPC only called on Clients
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

    /// <summary>
    /// Tries to play sound.
    /// </summary>
    /// <param name="name">Name of the Sound file, should be extracted from an XML!</param>
    public void PlayValveSound()
    {
        networkView.RPC("StartValveSound", RPCMode.All);
    }

    [RPC]
    public void StartValveSound()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag(Tags.player).Where(player => player.networkView.isMine))
        {
            if (player.GetComponent<Team>().ID != GetComponent<Team>().ID)
            {
                GameObject.Find("sounds_Vocal")
                          .GetComponent<AudioLibrary>()
                          .StartSound(
                              ValveState == ValveStates.Opened
                                  ? new XMLReader("GameSettings.xml").GetXML().GetElementsByTagName("valveLost")[0]
                                        .InnerText
                                  : new XMLReader("GameSettings.xml").GetXML().GetElementsByTagName("valveConquered")[0]
                                        .InnerText, 0f);
            }
            else
            {
                GameObject.Find("sounds_Vocal")
                          .GetComponent<AudioLibrary>()
                          .StartSound(
                              ValveState == ValveStates.Opened
                                  ? new XMLReader("GameSettings.xml").GetXML().GetElementsByTagName("valveConquered")[0]
                                        .InnerText
                                  : new XMLReader("GameSettings.xml").GetXML().GetElementsByTagName("valveLost")[0]
                                        .InnerText, 0f);
            }
        }
    }

    /// <summary>
    /// Tries to play sound.
    /// </summary>
    /// <param name="name">Name of the Sound file, should be extracted from an XML!</param>
    public void PlaySound(string name, float delay = 0f)
    {
        networkView.RPC("StartSound", RPCMode.All, name, delay);
    }

    [RPC]
    public void StartSound(string name, float delay)
    {
        if (soundLibrary == null)
            soundLibrary = transform.FindChild("sound_Valve").GetComponent<AudioLibrary>();
        soundLibrary.StartSound(name, delay);
    }

    /// <summary>
    /// Tries to stop sound.
    /// </summary>
    /// <param name="name">Name of the Sound file, should be extracted from an XML!</param>
    public void StopSound(string name)
    {
        networkView.RPC("EndSound", RPCMode.All, name);
    }

    [RPC]
    public void EndSound(string name)
    {
        if (soundLibrary == null)
            soundLibrary = transform.FindChild("sound_Valve").GetComponent<AudioLibrary>();
        soundLibrary.StopSound(name);
    }
}
