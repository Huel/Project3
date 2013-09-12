using UnityEngine;

public class MinionVisualsController : MonoBehaviour
{
    private Animator animator;

    public enum AnimStates{ Run, Dead, Attack, Push, Buff };
    private AnimStates state;

    private string run = "Run";
    private string dead = "Dead";
    private string attack = "Attack";
    private string attackType = "AttackType";
    private string push = "Push";
    private string buff = "Buff";

    private int randomAttack;
    private int lastAttack = 1;

    private bool[] checkChange = new bool[5];

    void Start()
    {
        for (int i = 1; i < 5; i++)
        {
            checkChange[i] = false;
        }
        animator = GetComponent<Animator>();
        animator.SetBool(run, true);
        checkChange[0] = true;
    }

    void Update()
    { 
        if (!networkView.isMine || gameObject == null)
            return;
        // not sure if necessary
        if (animator.GetBool(dead)) return;
        // ---------------------
        if (!GetComponent<Health>().IsAlive())
            state = AnimStates.Dead;

        else if (GetComponent<Skill>().State == SkillState.InExecution)
        {
            randomAttack = Random.Range(1, 4);
            state = AnimStates.Attack;
        }

        else if (GetComponent<MinionAgent>().getCurrentTargetType() == TargetType.Valve
            && GetComponent<MinionAgent>().getTarget().GetComponent<WorkAnimation>().Move(gameObject))
            state = AnimStates.Push;

        else if (GetComponent<MinionLamp>().getSwitchOn())
            state = AnimStates.Buff;

        else
            state = AnimStates.Run;

        AnimStateToBoolean();
        checkChanges();
    }

    private void AnimStateToBoolean()
    {
        switch (state)
        {
            case AnimStates.Dead:
                animator.SetBool(run, false);
                animator.SetBool(dead, true);
                animator.SetBool(attack, false);
                animator.SetBool(push, false);
                animator.SetBool(buff, false);
                break;
            case AnimStates.Attack:

                animator.SetInteger(attackType, randomAttack);

                animator.SetBool(run, false);
                animator.SetBool(dead, false);
                animator.SetBool(attack, true);
                animator.SetBool(push, false);
                animator.SetBool(buff, false);
                break;
            case AnimStates.Push:
                animator.SetBool(run, false);
                animator.SetBool(dead, false);
                animator.SetBool(attack, false);
                animator.SetBool(push, true);
                animator.SetBool(buff, false);
                break;
            case AnimStates.Buff:
                animator.SetBool(run, false);
                animator.SetBool(dead, false);
                animator.SetBool(attack, false);
                animator.SetBool(push, false);
                animator.SetBool(buff, true);
                break;
            case AnimStates.Run:
                animator.SetBool(run, true);
                animator.SetBool(dead, false);
                animator.SetBool(attack, false);
                animator.SetBool(push, false);
                animator.SetBool(buff, false);
                break;
        }
    }

    private void checkChanges()
    {
        if (checkChange[0] != animator.GetBool(run)
            || checkChange[1] != animator.GetBool(attack)
            || checkChange[2] != animator.GetBool(dead)
            || checkChange[3] != animator.GetBool(push)
            || checkChange[4] != animator.GetBool(buff)
            || lastAttack != animator.GetInteger(attackType))
        {
            networkView.RPC("TransferAnimStates", RPCMode.OthersBuffered, animator.GetBool(run), animator.GetBool(attack), animator.GetBool(dead), animator.GetBool(push), animator.GetBool(buff), animator.GetInteger(attackType));
            checkChange[0] = animator.GetBool(run);
            checkChange[1] = animator.GetBool(attack);
            checkChange[2] = animator.GetBool(dead);
            checkChange[3] = animator.GetBool(push);
            checkChange[4] = animator.GetBool(buff);
            lastAttack = animator.GetInteger(attackType);
        }
    }

    [RPC]
    public void TransferAnimStates(bool first, bool second, bool third, bool fourth, bool fifth, int type)
    {
        if (animator != null)
        {
            animator.SetBool(run, first);
            animator.SetBool(attack, second);
            animator.SetBool(dead, third);
            animator.SetBool(push, fourth);
            animator.SetBool(buff, fifth);
            animator.SetInteger(attackType, type);
        }
       
    }
}
