using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : Skill
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



    public void Awake()
    {
        //debug = true;
        skillName = "Basic Attack";
        SetDuration(SkillState.InExecution, _castTime);
        SetDuration(SkillState.CoolingDown, _cooldownTime);
        _targetTypes = new List<TargetType>() {TargetType.Minion, TargetType.Hero};
        _targetTeams = new List<Team.TeamIdentifier>() {GetComponent<Team>().Other()};
        _damageComponent = GetComponent<Damage>();
        _animator = GetComponent<NetworkAnimator>();

    }

    public override bool Execute()
    {
        if(!Executable || !enabled)
            return false;
        _animator.PlayAnimation(skillName);
        SwitchState();
        return true;
    }

    protected override void OnActive()
    {
        Target contact = contactTrigger.GetContactByTypesAndTeam(_targetTypes, _targetTeams);
        if (contact != null)
        {
            if (contact.GetComponent<Target>().type == TargetType.Hero) 
                contact.GetComponent<LastHeroDamage>().SetSource(networkView.viewID);
            Health enemyHealth = contact.GetComponent<Health>();
            enemyHealth.DecHealth(_damageComponent.DefaultDamage);
        }
        SwitchState();
    }

}
