using System.Collections.Generic;
using UnityEngine;

public class Attack : Skill
{
    [SerializeField]
    private float _castTime = 0.3f;
    [SerializeField]
    private float _cooldownTime = 0.2f;

    private List<TargetType> _targetTypes;
    private List<Team.TeamIdentifier> _targetTeams;
    private Damage _damageComponent;
    private NetworkAnimator _animator;
    public ContactTrigger contactTrigger;
    private Target _contact;



    public void Awake()
    {
        debug = true;
        skillName = "Attack";
        SetDuration(SkillState.InExecution, _castTime);
        SetDuration(SkillState.CoolingDown, _cooldownTime);
        _targetTypes = new List<TargetType>() { TargetType.Minion, TargetType.Hero };
        _targetTeams = new List<Team.TeamIdentifier>() { GetComponent<Team>().Other() };
        _damageComponent = GetComponent<Damage>();
        _animator = GetComponent<NetworkAnimator>();

    }

    public override bool Execute()
    {
        if (!Executable)
            return false;
        _contact = contactTrigger.GetContactByTypesAndTeam(_targetTypes, _targetTeams);
        if (_contact == null)
            return false;
        SwitchState();
        return true;
    }

    protected override void OnActive()
    {
        if (_contact != null)
        {
            Health enemyHealth = _contact.GetComponent<Health>();
            enemyHealth.DecHealth(_damageComponent.DefaultDamage);
        }
        SwitchState();
    }

}
