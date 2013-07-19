using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MinionAgent : MonoBehaviour
{
    public enum LaneIdentifier{ Lane1, Lane2, Lane3 };
    public LaneIdentifier laneID;

    private NavMeshAgent agent;

    public Target _destination;    // default target
    public Target _origin;         // came from here
    public Target _target;         // current target

    public Range attentionRange;
    public Range looseAttentionRange;
    public ContactTrigger contact;

    public bool forward;

    //public Skill basicSkill;

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        //if (networkView.isMine) 
        //    agent.enabled = true;
        //else
        //    return;
    }

    void Update()
    {
        //if (!networkView.isMine)
        //    return;
        if (_target == null)
        {
            agent.enabled = true;
            if (_destination != null)
                agent.destination = _destination.gameObject.transform.position;
            if (attentionRange != null)
               SelectTarget();
        }
        else if (agent.enabled)
            agent.destination = _target.gameObject.transform.position;
    }

    void SelectTarget()
    {
        Team team;
        var target = attentionRange.GetNearestTargetByType(TargetType.Minion);
        if (target != null)
        {
            team = target.gameObject.GetComponent<Team>();
            if (team != null && team.isEnemy(GetComponent<Team>()))
            {
                _target = target;
                contact.AddListener(target, OnEnemyContact);
                return;
            }
        }
       
        target = attentionRange.GetNearestTargetByType(TargetType.Hero);
        if (target != null)
        {
            team = target.gameObject.GetComponent<Team>();
            if (team != null && team.isEnemy(GetComponent<Team>()))
            {
                _target = target;
                contact.AddListener(target, OnEnemyContact);
                return;
            }
        }

        target = attentionRange.GetNearestTargetByType(TargetType.Valve);
        if (target != null)
        {
            _target = target;
            return;
        }

        target = attentionRange.GetNearestTargetByType(TargetType.Checkpoint);
        if (target != null)
        {
            _target = target;
            return;
        }
    }

    private void OnEnemyContact(Target target)
    {
        agent.enabled = false;
        //basicSkill.Execute();
    }
}
