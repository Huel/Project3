using UnityEngine;

public class MinionVisualsController : MonoBehaviour
{
    private Animator animator;
    private string run = "Run";
    private string dead = "Dead";
    private string attack = "Attack";
    private string attackType = "AttackType";
    private bool[] checkChange = new bool[3];

    void Start()
    {
        for (int i = 1; i < 3; i++)
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

        if (animator.GetBool(run) && GetComponent<Skill>().State == SkillState.InExecution)
        {
            int attackValue = (Random.Range(1, 3));
            animator.SetInteger(attackType, attackValue);

            animator.SetBool(run, false);
            animator.SetBool(attack, true);
        }

        if (animator.GetBool(attack) && GetComponent<Skill>().State == SkillState.OnCooldown)
        {
            animator.SetBool(attack, false);
            animator.SetBool(run, true);
        }

        if (!GetComponent<Health>().IsAlive())
        {
            animator.SetBool(attack, false);
            animator.SetBool(run, false);
            animator.SetBool(dead, true);
        }
        if (checkChange[0] != animator.GetBool(run) || checkChange[1] != animator.GetBool(attack) || checkChange[2] != animator.GetBool(dead))
        {
            networkView.RPC("TransferAnimStates", RPCMode.OthersBuffered, animator.GetBool(run), animator.GetBool(attack), animator.GetBool(dead), animator.GetInteger(attackType));
            checkChange[0] = animator.GetBool(run);
            checkChange[1] = animator.GetBool(attack);
            checkChange[2] = animator.GetBool(dead);
        }
    }

    [RPC]
    public void TransferAnimStates(bool first, bool second, bool third, int type)
    {
        animator.SetBool(run, first);
        animator.SetBool(attack, second);
        animator.SetBool(dead, third);
        animator.SetInteger(attackType, type);
    }
}
