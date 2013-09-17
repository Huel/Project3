using System.Collections.Generic;
using UnityEngine;

public class FreshMeat : Skill
{
    [SerializeField]
    private float _cooldownTime = 90f;
    [SerializeField]
    private bool _eatOwnMinions;
    [SerializeField]
    private bool _eatEnemyMinions = true;

    private List<TargetType> _targetTypes;
    private List<Team.TeamIdentifier> _targetTeams;
    private Target _contact;
    private Health _healthComponent;
    private NetworkAnimator _animator;
    public ContactTrigger contactTrigger;



    public void Awake()
    {
        debug = true;
        skillName = "Fresh Meat";
        SetDuration(SkillState.CoolingDown, _cooldownTime);
        SetStateCycle(new[] { SkillState.Ready, SkillState.Active, SkillState.CoolingDown });
        _healthComponent = GetComponent<Health>();
        _animator = GetComponent<NetworkAnimator>();
        _targetTypes = new List<TargetType>() { TargetType.Minion };
        _targetTeams = new List<Team.TeamIdentifier>();
        if (_eatOwnMinions) _targetTeams.Add(GetComponent<Team>().ID);
        if (_eatEnemyMinions) _targetTeams.Add(GetComponent<Team>().Other());

    }

    public override bool Execute()
    {
        if (!Executable)
            return false;
        _contact = contactTrigger.GetContactByTypesAndTeam(_targetTypes, _targetTeams);
        if (!_contact)
            return false;
        _animator.PlayAnimation(skillName);
        SwitchState();
        return true;
    }

    protected override void OnActive()
    {
        _contact.GetComponent<Health>().SetToMinHealth();
        _healthComponent.IncHealth(_healthComponent.MaxHealth / 2f);
        SwitchState();
        _contact = null;
    }

}
