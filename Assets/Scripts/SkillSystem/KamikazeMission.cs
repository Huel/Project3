using System.Collections.Generic;
using UnityEngine;

public class KamikazeMission : Skill
{
    [SerializeField]
    private float _cooldownTime = 20f;

    private TargetType _targetType;
    private Team _targetTeam;
    private Target _contact;
    private NetworkAnimator _animator;
    public Range _range;

    public void Awake()
    {
        debug = false;
        skillName = "Kamikaze Mission";
        SetDuration(SkillState.CoolingDown, _cooldownTime);
        SetStateCycle(new[] { SkillState.Ready, SkillState.Active, SkillState.CoolingDown });
        _targetType = TargetType.Minion;
        _targetTeam = GetComponent<Team>();
        _animator = GetComponent<NetworkAnimator>();
    }

    public override bool Execute()
    {
        if (!Executable)
            return false;
        _contact = _range.GetNearestTargetByTypeAndTeam(_targetType, _targetTeam);
        if (!_contact)
            return false;
        _animator.PlayAnimation(skillName);
        SwitchState();
        return true;
    }

    protected override void OnActive()
    {
        _contact.gameObject.GetComponent<MinionAgent>().Manipulate((int)ManipulateStates.Target, networkView.viewID, "Base");
        _contact = null;
        SwitchState();
    }
}
