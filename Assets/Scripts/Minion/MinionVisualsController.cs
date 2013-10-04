using System;
using UnityEngine;

public class MinionVisualsController : MonoBehaviour
{
    // Components
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    public enum AnimStates {Dead, Push, Buff };
    private AnimStates state;

    // Animator parameters
    private const string dying = "Dying";
    private const string attackType = "AttackType";
    private const string push = "Push";
    private const string buff = "Buff";
    private const string speed = "Speed";

    private float currentSpeed;
    private const float speedMultiplier = 0.333f;

    private Vector3 previousPosition;

    private bool debug = false;

    private bool[] checkChange = new bool[3];

    void Start()
    {
        for (int i = 1; i < 3; i++)
        {
            checkChange[i] = false;
        }
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void getCurrentSpeed()
    {
        Vector3 curMove = transform.position - previousPosition;
        currentSpeed = (curMove.magnitude / Time.deltaTime) * speedMultiplier;
        previousPosition = transform.position;
    }

    [RPC]
    public void setSpeed()
    {
        if (networkView.isMine)
            _animator.SetFloat(speed, currentSpeed);
        else
            networkView.RPC("setSpeed", RPCMode.OthersBuffered);
    }

    void Update()
    {
        if (!networkView.isMine || gameObject == null)
            return;
        // not sure if necessary
        if (_animator.GetBool(dying)) return;
        // ---------------------
        
        getCurrentSpeed();
        setSpeed();
        if (debug)
            DebugStreamer.message = networkView.viewID + "  speed: " + currentSpeed;

        if (!GetComponent<Health>().IsAlive())
            state = AnimStates.Dead;

        else if (GetComponent<MinionAgent>().GetCurrentTargetType() == TargetType.Valve
            && GetComponent<MinionAgent>().GetTarget().GetComponent<WorkAnimation>().Move(gameObject))
            state = AnimStates.Push;

        else if (GetComponent<MinionLamp>().getSwitchOn())
            state = AnimStates.Buff;

        AnimStateToBoolean();
        CheckChanges();
    }

    private void AnimStateToBoolean()
    {
        switch (state)
        {
            case AnimStates.Dead:
                _animator.SetBool(dying, true);
                _animator.SetBool(push, false);
                _animator.SetBool(buff, false);
                break;
            case AnimStates.Push:
                _animator.SetBool(dying, false);
                _animator.SetBool(push, true);
                _animator.SetBool(buff, false);
                break;
            case AnimStates.Buff:
                _animator.SetBool(dying, false);
                _animator.SetBool(push, false);
                _animator.SetBool(buff, true);
                break;
        }
    }

    private void CheckChanges()
    {
        if (checkChange[0] != _animator.GetBool(dying)
            || checkChange[1] != _animator.GetBool(push)
            || checkChange[2] != _animator.GetBool(buff))
        {
            networkView.RPC("TransferAnimStates", RPCMode.OthersBuffered, _animator.GetBool(dying), _animator.GetBool(push), _animator.GetBool(buff));
            checkChange[0] = _animator.GetBool(dying);
            checkChange[1] = _animator.GetBool(push);
            checkChange[2] = _animator.GetBool(buff);
        }
    }

    [RPC]
    public void TransferAnimStates( bool die, bool pushing, bool buffing)
    {
        if (_animator != null)
        {
            _animator.SetBool(dying, die);
            _animator.SetBool(push, pushing);
            _animator.SetBool(buff, buffing);
        }

    }
}
