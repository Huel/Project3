using System.Collections.Generic;
using UnityEngine;

public class ShieldWall : Skill
{
    [SerializeField]
    private float _duration = 5f;
    [SerializeField]
    private float _cooldownTime = 30f;
    [SerializeField]
    private bool _deactivatable;
    [SerializeField]
    private float _speedMultiplier = 0.5f;    


    private List<TargetType> _targetTypes;
    private List<Team.TeamIdentifier> _targetTeams;
    private Health _healthComponent;
    private Speed _speedComponent;
    private NetworkAnimator _animator;
    private bool _activeEffect;



    public void Awake()
    {
        debug = true;
        skillName = "Shieldwall";
        SetDuration(SkillState.CoolingDown, _cooldownTime);
        SetDuration(SkillState.Active, _duration);
        SetStateCycle(new[] { SkillState.Ready, SkillState.Active, SkillState.CoolingDown });
        _healthComponent = GetComponent<Health>();
        _speedComponent = GetComponent<Speed>();
        _animator = GetComponent<NetworkAnimator>();
        _targetTypes = new List<TargetType>() { TargetType.Minion };
        _targetTeams = new List<Team.TeamIdentifier>(){GetComponent<Team>().Other()};

    }

    public override bool Execute()
    {
        if (_deactivatable && _activeEffect)
        {
            SwitchState();
            return true;
        }
        if (!Executable)
            return false;


        _animator.PlayAnimation(skillName);
        SwitchState();
        return true;
    }

    protected override void OnActive()
    {
        if(_activeEffect)
            return;
        _healthComponent.SetInvulnerability(true);
        _speedComponent.SetSpeedMultiplier(_speedMultiplier);
        if (debug)
            DebugStreamer.message = "Shield wall is active.";
        _activeEffect = true;
    }

    protected override void OnCoolDown()
    {        
        if (!_activeEffect)
            return;
        _healthComponent.SetInvulnerability(false);
        _speedComponent.SetSpeedMultiplier(1.0f);
        if (debug)
            DebugStreamer.message = "Shield wall is inactive.";
        _activeEffect = false;
    }

}
