using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MinionAgent : MonoBehaviour
{
    public enum LaneIdentifier{ Lane1, Lane2, Lane3 };
    public LaneIdentifier laneID;

    private NavMeshAgent _agent;
    private Skill _basicAttack;

    public Target _destination;    // default target
    public Target _origin;         // came from here
    public Target _target;         // current target

    public Range attentionRange;
    public Range looseAttentionRange;
    public ContactTrigger contact;

    //public Skill basicSkill;

    void Start()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _basicAttack = gameObject.AddComponent<Skill>();
        _basicAttack.Init("Basic Attack");

        attentionRange.SetActive(_target == null);
        looseAttentionRange.SetActive(_target != null);
        contact.SetActive(_target != null);

        // only works when Minion is creatd by network.instansiate !!!
        // ***********************************************************
        //_agent.enabled = networkView.isMine;
        //this.enabled = networkView.isMine;
        //if (!networkView.isMine)
        //{
        //    attentionRange.SetActive(false);
        //    looseAttentionRange.SetActive(false);
        //    contact.SetActive(false);
        //}
        // ***********************************************************
    } 

    void Update()
    {
        // only works when Minion is creatd by network.instansiate !!!
        // ***********************************************************
        //if (!networkView.isMine)
        //    return;
        // ***********************************************************

        attentionRange.SetActive(_target == null);
        looseAttentionRange.SetActive(_target != null);
        contact.SetActive(_target != null);

        _agent.speed = gameObject.GetComponent<Speed>().CurrentSpeed;
        
        if (_target == null)
        {
            _agent.enabled = true;
            if (_destination != null)
                _agent.destination = _destination.gameObject.transform.position;
            else
                _agent.destination = transform.position;
            if (attentionRange != null && attentionRange.gameObject.active)
            {
                SelectTarget();
            }        
        }
        else
        {
            if (looseAttentionRange != null && looseAttentionRange.gameObject.active)
                if (!looseAttentionRange.isInRange(_target))
                {
                    _target = null;
                    return;
                }
            if (_agent.enabled)
                _agent.destination = _target.gameObject.transform.position;
            if (gameObject.GetComponent<Team>().ID != _target.gameObject.GetComponent<Team>().ID)
                _basicAttack.Execute();  // --> start fight ??
        }
        
            
    }

    void SelectTarget()
    {
        var target = attentionRange.GetNearestTargetByTypePriorityAndEnemyOnly(new List<TargetType> { TargetType.Minion, TargetType.Hero, TargetType.Valve, TargetType.Checkpoint }, GetComponent<Team>());
        if (target == null) return;
        if (target.type == TargetType.Minion || target.type == TargetType.Hero)
        {
            _target = target;
            contact.AddListener(target, OnEnemyContact);
            return;
        }

        if (target.type == TargetType.Valve)
        {
            _target = target;
            return;
        }

        if (target.type == TargetType.Checkpoint)
        {
            _target = target;
        }
    }

    private void OnEnemyContact(Target target)
    {
        //_agent.enabled = false;
        //basicSkill.Execute();
    }
}
