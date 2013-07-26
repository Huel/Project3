using System.Collections.Generic;
using UnityEngine;

public class MinionAgent : MonoBehaviour
{
    public enum LaneIdentifier { Lane1, Lane2, Lane3 };
    public LaneIdentifier laneID;

    private NavMeshAgent agent;
    private Skill basicAttack;

<<<<<<< HEAD
    public Target _destination;    // default target
    public Target _origin;         // came from here
    public Target _target;         // current target
    private float destinationOffset = 1f;
=======
    private Target _destination;    // default target
    private Target _origin;         // came from here
    private Target _target;         // current target
    private float _destinationOffset = 1f;
>>>>>>> refs/heads/feature/optimize_collider'n'stuff

    public Range attentionRange;
    public Range looseAttentionRange;
    public ContactTrigger contact;

    //public Skill basicSkill;

    void Start()
    {
<<<<<<< HEAD
        agent = gameObject.GetComponent<NavMeshAgent>();
        basicAttack = gameObject.AddComponent<Skill>();
        basicAttack.Init("Basic Attack");

        if (networkView.isMine)
            agent.enabled = true;
    }

    void Update()
    {
        if (!networkView.isMine)
            this.enabled = false;
        agent.speed = gameObject.GetComponent<Speed>().CurrentSpeed;
=======
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _basicAttack = gameObject.AddComponent<Skill>();
        _basicAttack.Init("Basic Attack");

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

        attentionRange.SetActive(_target == null);
        looseAttentionRange.SetActive(_target != null);
        contact.SetActive(_target != null);

        _agent.speed = gameObject.GetComponent<Speed>().CurrentSpeed;
        
        // no target
>>>>>>> refs/heads/feature/optimize_collider'n'stuff
        if (_target == null)
        {
            agent.enabled = true;
            if (_destination != null)
            {
<<<<<<< HEAD
                agent.destination = _destination.Position;
                if (_destination.GetDistance(transform.position) <= destinationOffset)
=======
                _agent.destination = _destination.Position;
                if (_destination.GetDistance(transform.position) <= _destinationOffset)
>>>>>>> refs/heads/feature/optimize_collider'n'stuff
                {
                    CheckPoint checkPoint = _destination.gameObject.GetComponent<CheckPoint>();
                    if (checkPoint != null)
                    {
                        Target newDestination = checkPoint.GetNextCheckPoint(_origin);
                        SetOrigin(_destination);
                        SetDestination(newDestination);
<<<<<<< HEAD

                    }
                    else
                    {
                        SetDestination(null);
                    }
                }

            }
            else
            {
                //if it hasn't a destination reset to its position
                agent.destination = transform.position;
            }
            if (attentionRange != null)
                SelectTarget();
=======
                    }
                    else
                        SetDestination(null);
                }

            }
            else  //move to own position
                _agent.destination = transform.position;
            if (attentionRange != null && attentionRange.gameObject.active)
                SelectTarget();       
>>>>>>> refs/heads/feature/optimize_collider'n'stuff
        }
        else if (agent.enabled)
            agent.destination = _target.gameObject.transform.position;
        if (looseAttentionRange != null && _target != null)
            if (!looseAttentionRange.isInRange(_target))
            {
                _target = null;

            }
        if (_target != null)
            basicAttack.Execute();
    }

    void SelectTarget()
    {
        Team team;
        var target = attentionRange.GetNearestTargetByTypePriorityAndEnemyOnly(new List<TargetType> { TargetType.Minion, TargetType.Hero, TargetType.Valve, TargetType.Checkpoint }, GetComponent<Team>());
        if (target == null) return;

        if (target.type == TargetType.Minion || target.type == TargetType.Hero)
        {
            team = target.gameObject.GetComponent<Team>();
            if (team != null && team.isEnemy(GetComponent<Team>()))
            {
                _target = target;
                contact.AddListener(target, OnEnemyContact);
                return;
            }
        }

        if (target.type == TargetType.Valve)
        {
            _target = target;
            return;
        }
    }

    private void OnEnemyContact(Target target)
    {
        //agent.enabled = false;
        //basicSkill.Execute();
    }

    public void SetDestination(Target destination)
    {
        _destination = destination;
    }

    public void SetOrigin(Target origin)
    {
        _origin = origin;
    }
}
