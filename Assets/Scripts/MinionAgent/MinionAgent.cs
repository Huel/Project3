using System.Collections.Generic;
using UnityEngine;

public class MinionAgent : MonoBehaviour
{
    public enum LaneIdentifier { Lane1, Lane2, Lane3 };
    public LaneIdentifier laneID;

    private NavMeshAgent _agent;
    public Skill basicAttack;

    public Target _destination;    // default target
    private Target _origin;         // came from here
    public Target _target;         // current target
    private Target _targetSaved;         // current target (Saved for overwritting processces)
    private float _destinationOffset = 1f;

    public Range attentionRange;
    public Range looseAttentionRange;
    public ContactTrigger contact;

    public float productivity = 1f;

    //public Skill basicSkill;

    void Start()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();

        attentionRange.SetActive(_target == null);
        looseAttentionRange.SetActive(_target != null);
        contact.SetActive(_target != null);

        // only works when Minion is creatd by network.instansiate !!!
        // ***********************************************************
        _agent.enabled = networkView.isMine;
        this.enabled = networkView.isMine;
        if (!networkView.isMine)
        {
            attentionRange.SetActive(false);
            looseAttentionRange.SetActive(false);
            contact.SetActive(false);
        }
        // ***********************************************************
    }

    void Update()
    {
        // only works when Minion is creatd by network.instansiate !!!
        // ***********************************************************
        if (!networkView.isMine)
            return;
        // ***********************************************************
        if (!gameObject.GetComponent<Health>().IsAlive())
        {
            attentionRange.SetActive(false);
            looseAttentionRange.SetActive(false);
            contact.SetActive(false);
            _agent.enabled = false;
        }
        attentionRange.SetActive(_target == null);
        looseAttentionRange.SetActive(_target != null);
        contact.SetActive(_target != null);

        _agent.speed = gameObject.GetComponent<Speed>().CurrentSpeed;

        // no target
        if (_target == null)
        {
            _agent.enabled = true;
            if (_destination != null)
            {
                _agent.destination = _destination.Position;
                if (_destination.GetDistance(transform.position) <= _destinationOffset)
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
            else  //move to own position
                _agent.destination = transform.position;
            if (attentionRange != null && attentionRange.gameObject.activeSelf)
                SelectTarget();
        }
        // already has target
        else
        {
            //if target out of range
            if (looseAttentionRange != null && looseAttentionRange.gameObject.activeSelf
                && !looseAttentionRange.isInRange(_target))
            {
                _target = null;
                return;
            }
            if (_agent.enabled)
                _agent.destination = _target.gameObject.transform.position;

        }
        if (contact.gameObject.activeSelf && !_agent.enabled)
        {
            _agent.enabled = true;
        }
        if (_target != null && contact.Contact(_target))
        {
            basicAttack.Execute();
        }
        if (_target != null && _target.type == TargetType.Dead)
        {
            _target = null;
        }
    }

    void SelectTarget()
    {
        var target = attentionRange.GetNearestTargetByTypePriorityAndEnemyOnly(new List<TargetType> { TargetType.Minion, TargetType.Hero, TargetType.Valve, TargetType.Checkpoint }, GetComponent<Team>());
        if (target == null) return;
        if (target.type == TargetType.Minion || target.type == TargetType.Hero)
        {
            _target = target;
            return;
        }

        if (target.type == TargetType.Valve)
        {
            _target = target;
            return;
        }
    }

    public void SetDestination(Target destination)
    {
        _destination = destination;
    }

    public void SetOrigin(Target origin)
    {
        _origin = origin;
    }

    public Target GetDestination()
    {
        return _destination;
    }

    public Target GetOrigin()
    {
        return _origin;
    }

    public void Manipulate(string effect, string value="", Target aim=null)
    {
        switch (effect)
        {
            case "Target":
                _targetSaved = _target;
                _target = aim;
                break;

            case "ResetTarget":
                _target = _targetSaved;
                break;

            case "CanMove":
                _agent.enabled = bool.Parse(value);
                break;

            case "CanAttack":
                basicAttack.enabled = bool.Parse(value);
                break;

            case "Productivity":
                productivity = float.Parse(value);
                break;

            case "RelevantTargetTypes":
                List<TargetType> compareTypes = new List<TargetType> { TargetType.Hero, TargetType.Minion, TargetType.Spot, TargetType.Valve, TargetType.Dead };
                List<TargetType> types = new List<TargetType>();
                string[] splitedString = value.Split(new string[] { ", " }, System.StringSplitOptions.None);
                foreach (string type in splitedString)
                    foreach (TargetType compareType in compareTypes)
                        if (compareType.ToString() == type)
                            types.Add(compareType);
                attentionRange.SetRelevantTargetTypes(types);
                break;

            case "Revive":
                gameObject.GetComponent<Health>().SetHealth(gameObject.GetComponent<Health>().MaxHealth * float.Parse(value));
                break;
        }
    }
}
