using System;
using UnityEngine;


public abstract class Skill : MonoBehaviour
{
    public enum SkillState { Ready, InExecution, Active, CoolingDown }
    protected delegate void OnState();

    private SkillState _state = SkillState.Ready;
    private float _stateTime;
    private readonly float[] _stateDuration = { 0f, 0f, 0f, 0f };
    private SkillState _nextState;



    protected SkillState[] _stateCycle = { SkillState.Ready, SkillState.InExecution, SkillState.CoolingDown };


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
        get { return (State == SkillState.Ready) && (!ExecutingSkill()); }
    }

    void Start()
    {
        _nextState = State;
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
            if (skill.State == SkillState.InExecution)
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
        State = _nextState;
    }

}