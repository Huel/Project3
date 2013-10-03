using System.Collections.Generic;
using UnityEngine;

public class BattleCry : Skill
{
    [SerializeField]
    private float _duration = 3f;
    [SerializeField]
    private float _cooldownTime = 20f;

    private List<Target> _contacts;
    private List<TargetType> _targetTypes;
    private List<Team.TeamIdentifier> _targetTeams;
    private NetworkAnimator _animator;
    public Range _range;
    private bool _activeEffect;

    public void Awake()
    {
        debug = false;
        skillName = "Battlecry";
        SetDuration(SkillState.CoolingDown, _cooldownTime);
        SetDuration(SkillState.Active, _duration);
        SetStateCycle(new[] { SkillState.Ready, SkillState.Active, SkillState.CoolingDown });
        _targetTypes = new List<TargetType>() { TargetType.Minion };
        _targetTeams = new List<Team.TeamIdentifier>() { GetComponent<Team>().Other() };
        _animator = GetComponent<NetworkAnimator>();
    }

    public override bool Execute()
    {
        if (_activeEffect)
            return false;

        if (!Executable)
            return false;

        _contacts = _range.GetTargetsByTypesAndTeam(_targetTypes, _targetTeams);
        _animator.PlayAnimation(skillName);
        SwitchState();
        return true;
    }

    protected override void OnActive()
    {
        if (_activeEffect)
            return;
        _activeEffect = true;

        if (_contacts.Count == 0) return;
        foreach (Target target in _contacts)
        {
            if (Random.Range(0f, 1f) <= 0.5f)
                target.gameObject.GetComponent<MinionAgent>().Manipulate((int)ManipulateStates.Target, networkView.viewID, "Flee");
            else
                target.gameObject.GetComponent<MinionAgent>().Manipulate((int)ManipulateStates.Movement, networkView.viewID, "false");
        }
    }

    protected override void OnCoolDown()
    {
        if (!_activeEffect)
            return;
        _activeEffect = false;

        if (_contacts.Count == 0) return;
        foreach (Target target in _contacts)
        {
            target.gameObject.GetComponent<MinionAgent>().Manipulate((int)ManipulateStates.ResetTarget, networkView.viewID, "");
            target.gameObject.GetComponent<MinionAgent>().Manipulate((int)ManipulateStates.Movement, networkView.viewID, "true");
        }
        _contacts = null;
    }
}
