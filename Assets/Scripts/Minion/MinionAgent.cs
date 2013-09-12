using System.Collections.Generic;
using UnityEngine;

public class MinionAgent : MonoBehaviour
{
    public enum LaneIdentifier { Lane1, Lane2, Lane3 };
    public LaneIdentifier laneID;

    private NavMeshAgent _agent;
    public Skill basicAttack;

    public Target _destination;    // default target
    public Target _destinationSaved;    // default target (Saved for overwritting processces)
    private Target _origin;         // came from here
    public Target _target;         // current target
    private Target _targetSaved;         // current target (Saved for overwritting processces)
    private float _destinationOffset = 1f;

    private bool fixedTarget = false;
    public bool atRallyPoint = false;
    public bool partOfSquad = false;

    public Range attentionRange;
    public Range looseAttentionRange;
    public ContactTrigger contact;

    public float productivity = 2f;

    private float life;
    private float oldLife;

    public bool _moving;

    public Target getTarget() { return _target; }

    public TargetType getCurrentTargetType()
    {
        if (_target != null) return _target.type;
        return TargetType.None;
    }

    //public Skill basicSkill;

    void Start()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();

        attentionRange.SetActive(_target == null);
        looseAttentionRange.SetActive(_target != null);
        contact.SetActive(_target != null);

        life = gameObject.GetComponent<Health>().HealthPoints;

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
            return;
        }

        oldLife = life;
        life = gameObject.GetComponent<Health>().HealthPoints;

        attentionRange.SetActive(_target == null);
        looseAttentionRange.SetActive(_target != null);
        contact.SetActive(_target != null);

        _agent.speed = gameObject.GetComponent<Speed>().CurrentSpeed;

        CheckRallyPoint();
        CheckAttacked();
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
            if (_target.type == TargetType.Valve && _target.gameObject.GetComponent<Valve>().stateComplete(this))
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
        if (target.type == TargetType.Valve && target.gameObject.GetComponent<Valve>().isAvailable(this))
            _target = target;
    }

    void ContactBehavior()
    {
        if (contact.Contact(_target) && _target != null)
        {
            if (_agent.enabled) _agent.Stop(true);
            transform.LookAt(_target.transform);
            // Enemy
            if (_target.type == TargetType.Hero || _target.type == TargetType.Minion)
                basicAttack.Execute();
            // Valve
            else if (_target.type == TargetType.Valve && _target.GetComponent<Valve>().isAvailable(this))
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

    void CheckAttacked()
    {
        if (oldLife > life && (_target == null || _target.type == TargetType.Spot || _target.type == TargetType.Valve) && !fixedTarget)
        {
            var target = attentionRange.GetNearestTargetByPriority(new List<TargetType> { TargetType.Minion, TargetType.Hero }, gameObject.GetComponent<Team>(), true);
            if (target != null) _target = target;
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


    public void Manipulate(string effect, string value = "", Target target = null)
    {
        NetworkViewID aim;
        if (target == null)
            aim = NetworkViewID.unassigned;
        else
            aim = target.networkView.viewID;
        if (networkView.isMine)
            ExecuteManipulation(effect, value, aim);
        else
            networkView.RPC("ExecuteManipulation", networkView.owner, effect, value, aim);
    }

    [RPC]
    private void ExecuteManipulation(string effect, string value, NetworkViewID aim)
    {
        Target target = null;
        if (aim != NetworkViewID.unassigned)
            target = NetworkView.Find(aim).GetComponent<Target>();

        switch (effect)
        {
            case "Target":
                _targetSaved = _target;
                _target = target;
                fixedTarget = true;
                break;

            case "ResetTarget":
                _target = _targetSaved;
                fixedTarget = false;
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
                List<TargetType> compareTypes = new List<TargetType>
                    {
                        TargetType.Hero,
                        TargetType.Minion,
                        TargetType.Spot,
                        TargetType.Valve,
                        TargetType.Dead
                    };
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

            case "AddSquad":
                if (atRallyPoint)
                    atRallyPoint = false;
                else
                    _destinationSaved = _destination;
                _destination = target;
                partOfSquad = true;
                break;

            case "RemoveSquad":
                _destination = CheckLine(_destinationSaved);
                _destinationSaved = null;
                partOfSquad = false;
                break;
        }
    }

    private Target CheckLine(Target oldLine)
    {
        List<GameObject> checkpoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Checkpoint"));

        Target target;
        float distance = float.MaxValue;
        int position = -1;
        for (int i = 0; i < checkpoints.Count; i++)
            if ((checkpoints[i].gameObject.transform.position - gameObject.transform.position).magnitude <= distance)
            {
                position = i;
                distance = (checkpoints[i].gameObject.transform.position - gameObject.transform.position).magnitude;
            }

        if (position != -1)
        {
            target = checkpoints[position].GetComponent<Target>();
            Team.TeamIdentifier id = GetComponent<Team>().ID;
            int number = int.Parse(target.name.Substring(target.name.Length - 1));
            if ((number == 3 && id == Team.TeamIdentifier.Team2) || (number == 1 && id == Team.TeamIdentifier.Team1))
            {
                foreach (GameObject baseObject in GameObject.FindGameObjectsWithTag("Base"))
                    if (baseObject.GetComponent<Team>().ID == id)
                        SetOrigin(baseObject.GetComponent<Target>());
            }
            number = (id == Team.TeamIdentifier.Team1) ? number - 1 : number + 1;

            foreach (GameObject checkpoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
                if (checkpoint.name == (target.name.Substring(0, target.name.Length - 1) + number))
                    SetOrigin(checkpoint.GetComponent<Target>());
        }
        else
            target = oldLine;

        return target;
    }

    [RPC]
    public void SetNavMeshAgent(bool active)
    {
        GetComponent<NavMeshAgent>().enabled = active;
    }
}
