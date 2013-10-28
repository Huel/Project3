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
    private const string scared = "Scared";

    private float currentSpeed;
    private const float speedMultiplier = 0.333f;

    private Vector3 previousPosition;

    public bool debug = false;

    private bool[] checkChange = new bool[4];

    void Start()
    {
        for (int i = 1; i < 4; i++)
        {
            checkChange[i] = false;
        }
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void getCurrentSpeed()
    {
        if (_animator.GetBool(push))
        {
            currentSpeed = 1f;
            return;
        }
        Vector3 curMove = transform.position - previousPosition;
        currentSpeed = (curMove.magnitude / Time.deltaTime) * speedMultiplier;
        previousPosition = transform.position;
    }
    void Update()
    {
        getCurrentSpeed();
        _animator.SetFloat(speed, currentSpeed);
        if (debug)
            DebugStreamer.message = networkView.viewID + "  speed: " + currentSpeed;
        if (!networkView.isMine || gameObject == null)
            return;
        // not sure if necessary
        if (_animator.GetBool(dying)) return;
        // ---------------------
        if (GetComponent<ComponentBuilder>().state != ComponentBuilder.LoadingState.Loaded)
            return;

        _animator.SetBool(buff, !_animator.GetBool("Attack"));

        if (!GetComponent<Health>().IsAlive())
        {
            _animator.SetBool(dying, true);
            _animator.SetBool(push, false);
            _animator.SetBool(buff, false);
            _animator.SetBool(scared, false);
        }
        else if (GetComponent<MinionAgent>().GetCurrentTargetType() == TargetType.Valve
            && GetComponent<MinionAgent>().GetTarget().GetComponent<WorkAnimation>().Move(gameObject))
        {
            _animator.SetBool(dying, false);
            _animator.SetBool(push, true);
            _animator.SetBool(buff, false);
            _animator.SetBool(scared, false);
        }
        else if (GetComponent<MinionLamp>().getSwitchOn())
        {
            _animator.SetBool(dying, false);
            _animator.SetBool(push, false);
            _animator.SetBool(buff, true);
            _animator.SetBool(scared, false);
        }
        else if (GetComponent<MinionAgent>().isScared())
        {
            _animator.SetBool(dying, false);
            _animator.SetBool(push, false);
            _animator.SetBool(buff, false);
            _animator.SetBool(scared, true);
        }
        else
        {
            _animator.SetBool(dying, false);
            _animator.SetBool(push, false);
            _animator.SetBool(buff, false);
            _animator.SetBool(scared, false);
        }

        CheckChanges();
    }

    private void CheckChanges()
    {
        if (checkChange[0] != _animator.GetBool(dying)
            || checkChange[1] != _animator.GetBool(push)
            || checkChange[2] != _animator.GetBool(buff)
            || checkChange[3] != _animator.GetBool(scared))
        {
            networkView.RPC("TransferAnimStates", RPCMode.OthersBuffered, _animator.GetBool(dying), _animator.GetBool(push), _animator.GetBool(buff), _animator.GetBool(scared));
            checkChange[0] = _animator.GetBool(dying);
            checkChange[1] = _animator.GetBool(push);
            checkChange[2] = _animator.GetBool(buff);
            checkChange[3] = _animator.GetBool(scared);
        }
    }

    [RPC]
    public void TransferAnimStates( bool die, bool pushing, bool buffing, bool isScared)
    {
        if (_animator != null)
        {
            _animator.SetBool(dying, die);
            _animator.SetBool(push, pushing);
            _animator.SetBool(buff, buffing);
            _animator.SetBool(scared, isScared);
        }

    }
}
