using System.Collections.Generic;
using UnityEngine;

public enum ManipulateStates { Target, ResetTarget, Movement }

public class MinionAgent : MonoBehaviour
{
    public enum LaneIdentifier { Lane1, Lane2, Lane3 };
    public LaneIdentifier laneID;

    private NavMeshAgent _agent;
    public Skill basicAttack;

    private Target _destination;    // default target
    private Target _destinationSaved;    // default target (Saved for overwritting processces)
    private Target _origin;         // came from here
    private Target _target;         // current target
    private Target _targetSaved;         // current target (Saved for overwritting processces)
    
    private float _destinationOffset = 1f;
    public float productivity = 1f;

    private bool fixedTarget = false;
    public bool atRallyPoint = false;
    public bool partOfSquad = false;
    private bool _moving;
    private bool buff;

    private bool scared;

    public Range attentionRange;
    public Range looseAttentionRange;
    public ContactTrigger contact;

    public Target GetTarget() { return _target; }

    public void SetDestination(Target destination) { _destination = destination; }

    public void SetOrigin(Target origin) { _origin = origin; }

    public Target GetDestination() { return _destination; }

    public Target GetOrigin() { return _origin; }

    public TargetType GetCurrentTargetType()
    {
        if (_target != null) return _target.type;
        return TargetType.None;
    }

<<<<<<< HEAD
    public bool Buff
    {
        get { return buff; }
        set
        {
            if (value)
            {
                GetComponent<Health>().SetIncreasedMaxHealth(20);
                GetComponent<Damage>().SetIncreasedDamage(2);
                productivity = 1.2f;
            }
            else
            {
                GetComponent<Health>().SetIncreasedMaxHealth(0);
                GetComponent<Damage>().SetIncreasedDamage(0);
                productivity = 1.0f;
            }
            GetComponent<Speed>().IsSprinting = value;
            buff = value;
        }
    }
=======
    public bool isScared() { return scared; }
>>>>>>> refs/heads/feature/newMinionController

    void Start()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();

        attentionRange.SetActive(_target == null);
        looseAttentionRange.SetActive(_target != null);
        contact.SetActive(_target != null);
        scared = false;

        _agent.enabled = networkView.isMine;
        enabled = networkView.isMine;
        if (!networkView.isMine)
        {
            attentionRange.SetActive(false);
            looseAttentionRange.SetActive(false);
            contact.SetActive(false);
        }
    }

    void Update()
    {
        if (!networkView.isMine)
            return;

        if (!gameObject.GetComponent<Health>().IsAlive())
        {
            attentionRange.SetActive(false);
            looseAttentionRange.SetActive(false);
            contact.SetActive(false);
            _agent.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;  // RPC ???
            return;
        }

        attentionRange.SetActive(_target == null);
        looseAttentionRange.SetActive(_target != null);
        contact.SetActive(_target != null);

        _agent.speed = gameObject.GetComponent<Speed>().CurrentSpeed;

        CheckRallyPoint();
        TargetBehavior();
    }

    void TargetBehavior()
    {  
        // no target
        if (_target == null)
        {
            _moving = true;
            if (_agent.enabled) _agent.Resume();
            if (_destination != null)
            {
                if (_moving && _agent.enabled) _agent.destination = _destination.Position;
                if (_destination.GetDistance(transform.position) <= _destinationOffset && !partOfSquad)
                {
                    CheckPoint checkPoint = _destination.gameObject.GetComponent<CheckPoint>();
                    if (checkPoint != null)
                    {
                        Target newDestination = checkPoint.GetNextCheckPoint(_origin);
                        SetOrigin(_destination);
                        SetDestination(newDestination);
                    }
                    else
                        SetDestination(null);
                }
            }
            else if (_moving && _agent.enabled) //move to own position
                _agent.destination = transform.position;
            if (attentionRange != null && attentionRange.gameObject.activeSelf)
                SelectTarget();
        }
        // already has target
        else
        {
            //if target out of range
            if (!looseAttentionRange.isInRange(_target) && !fixedTarget)
            {
                _target = null;
                return;
            }
            //move to target
            if (_moving && _agent.enabled)
                _agent.destination = _target.gameObject.transform.position;
            //if valve is fully opened or closed
            if (_target.type == TargetType.Valve && _target.gameObject.GetComponent<Valve>().IsCompleted(this))
            {
                _target.GetComponent<Valve>().RemoveMinion(this);
                _target.GetComponent<WorkAnimation>().RemoveMinion(gameObject);
                _target = null;
            }
            ContactBehavior();
        }

        if (_target != null && _target.type == TargetType.Dead)
            _target = null;
    }

    void SelectTarget()
    {
        var target = attentionRange.GetNearestTargetByPriority(new List<TargetType> { TargetType.Minion, TargetType.Hero, TargetType.Valve }, gameObject.GetComponent<Team>());
        if (target == null) return;
        if (target.type == TargetType.Minion || target.type == TargetType.Hero)
        {
            _target = target;
            return;
        }
        if (target.type == TargetType.Valve && target.gameObject.GetComponent<Valve>().IsAvailable(this))
            _target = target;
    }

    void ContactBehavior()
    {
        if (contact.Contact(_target) && _target != null)
        { 
            if (_agent.enabled) _agent.Stop(true);
            
            // Enemy
            if (basicAttack && (_target.type == TargetType.Hero || _target.type == TargetType.Minion))
            {
                transform.LookAt(_target.transform);
                basicAttack.Execute();

                if (_target.type == TargetType.Minion)
                    _target.networkView.RPC("GetAttacked", _target.networkView.owner, gameObject.networkView.viewID);
            }
            // Valve
            else if (_target.type == TargetType.Valve && _target.GetComponent<Valve>().IsAvailable(this))
                _target.gameObject.GetComponent<Valve>().AddMinion(this);
            else
                return;
            _moving = false;
        }
        else
        {
            _moving = true;
            if (_agent.enabled) _agent.Resume();
        }
    }

    void CheckRallyPoint()
    {
        if (partOfSquad) return;
        var target = attentionRange.GetNearestTargetByTypeAndTeam(TargetType.Spot, gameObject.GetComponent<Team>());
        atRallyPoint = (target != null);
        if (atRallyPoint && _destination != null)
        {
            _destinationSaved = _destination;
            _destination = null;
        }
        if (!atRallyPoint && _destinationSaved != null)
        {
            _destination = _destinationSaved;
            _destinationSaved = null;
        }
    }

    [RPC]
    public void GetAttacked(NetworkViewID viewId)
    {
        if (!fixedTarget && _target != null && _target.type != TargetType.Hero && _target.type != TargetType.Minion)
            _target = NetworkView.Find(viewId).observed.gameObject.GetComponent<Target>();
    }

    [RPC]
    public void SetNavMeshAgent(bool active)
    {
        GetComponent<NavMeshAgent>().enabled = active;
    }

    [RPC]
    public void Manipulate(int state, NetworkViewID id, string value)
    {
        if (networkView.isMine)
        {
            switch ((ManipulateStates)state)
            {
                case (ManipulateStates.Target):
                    fixedTarget = true;
                    _targetSaved = _target;
                    if(value=="Base" || value=="Flee")
                    {
                        GameObject[] objects = GameObject.FindGameObjectsWithTag(Tags.baseArea);
                        foreach (GameObject gameObj in objects)
                        {
                            if (value=="Base" && gameObj.GetComponent<Team>().IsEnemy(GetComponent<Team>()))
                                _target = gameObj.GetComponent<Target>();
                            if (value == "Flee" && gameObj.GetComponent<Team>().IsOwnTeam(GetComponent<Team>()))
                            {
                                _target = gameObj.GetComponent<Target>();
                                scared = true;
                            }
                        }
                    }
                    else
                        _target = NetworkView.Find(id).GetComponent<Target>();
                    break;

                case (ManipulateStates.ResetTarget):
                    if (!fixedTarget) return;
                    fixedTarget = false;
                    scared = false;
                    _target = _targetSaved;
                    _targetSaved = null;
                    break;

                case (ManipulateStates.Movement):
                    
                    _agent.enabled = bool.Parse(value);
                    basicAttack.enabled = bool.Parse(value);
                    scared = !bool.Parse(value);
                    break;
            }
        }
        else
        {
            networkView.RPC("Manipulate", networkView.owner, (int)state, id, (value == "") ? "" : value);
        }
    }
}
