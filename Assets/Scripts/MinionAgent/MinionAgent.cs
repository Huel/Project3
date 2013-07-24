using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MinionAgent : MonoBehaviour
{
    public enum LaneIdentifier{ Lane1, Lane2, Lane3 };
    public LaneIdentifier laneID;

    private NavMeshAgent agent;
    private Skill basicAttack;

    public Target _destination;    // default target
    public Target _origin;         // came from here
    public Target _target;         // current target

    public Range attentionRange;
    public Range looseAttentionRange;
    public ContactTrigger contact;

    //public Skill basicSkill;

    void Start()
    {
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
        if (_target == null)
        {
            agent.enabled = true;
            if (_destination != null)
                agent.destination = _destination.gameObject.transform.position;
            else
            {
                //if it hasn't a destination reset to its position
                agent.destination = transform.position;
            }
            if (attentionRange != null)
                SelectTarget();
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

        if (target.type == TargetType.Checkpoint)
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
}
