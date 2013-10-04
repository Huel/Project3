using System;
using UnityEngine;


public abstract class Skill : MonoBehaviour
{
    public enum SkillState { Ready, InExecution, Active, CoolingDown }


    private float _stateTime;
    private float[] _stateDuration = { 0f, 0f, 0f, 0f };
    private SkillState[] _stateCycle = { SkillState.Ready, SkillState.InExecution, SkillState.Active, SkillState.CoolingDown };
    private SkillState _state = SkillState.Ready;
    private SkillState _nextState = SkillState.Ready;

    protected string skillName = "Skill";
    protected bool debug;

    public float getCooldownInPercent()
    {
        if (State == SkillState.CoolingDown)
            return _stateTime/_stateDuration[3];
        return 1;
    }

    public string SkillName
    {
        get { return skillName; }
    }

    public SkillState State
    {
        get { return _state; }
        private set
        {
            if (_state == value)
                return;
            _state = value;
            _stateTime = 0f;
        }
    }
    public float StateTime { get { return _stateTime; } }

    protected bool Executable
    {
        get
        {
            if (debug && !((State == SkillState.Ready) && (!ExecutingSkill())))
                DebugStreamer.message = skillName + " is not executable!";
            return (State == SkillState.Ready) && (!ExecutingSkill());
        }
    }

    private void UpdateStateTime(float deltaTime)
    {
        float duration = _stateDuration[(int)State];
        if (duration == 0f)
            return;
        _stateTime += deltaTime;
        if (_stateTime >= duration)
            SwitchState();

    }

    private bool ExecutingSkill()
    {
        Skill[] skills = GetComponents<Skill>();
        foreach (Skill skill in skills)
        {
            if (skill.State == SkillState.InExecution || skill.State == SkillState.Active)
                return true;
        }
        return false;
    }

    protected void SwitchState()
    {
        _nextState = _stateCycle[(Array.IndexOf(_stateCycle, State) + 1) % _stateCycle.Length];
    }

    protected void SetDuration(SkillState state, float duration)
    {
        _stateDuration[(int)state] = duration;
    }
    public float GetDuration(SkillState state)
    {
        return _stateDuration[(int)state];
    }
    protected void SetStateCycle(SkillState[] states)
    {
        _stateCycle = states;
    }

    public abstract bool Execute();

    protected virtual void OnExecute()
    {
    }
    protected virtual void OnActive()
    {
    }
    protected virtual void OnCoolDown()
    {
    }
    protected virtual void OnReady()
    {
    }

    void Update()
    {
        UpdateStateTime(Time.deltaTime);
        switch (State)
        {
            case SkillState.Ready:
                OnReady();
                break;
            case SkillState.InExecution:
                OnExecute();
                break;
            case SkillState.Active:
                OnActive();
                break;
            case SkillState.CoolingDown:
                OnCoolDown();
                break;
        }
        if (State != _nextState)
        {
            State = _nextState;
            if (debug)
                DebugStreamer.message = skillName + ": " + State;
        }
    }

}