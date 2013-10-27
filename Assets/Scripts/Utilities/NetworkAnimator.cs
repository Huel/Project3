using System.Collections;
using UnityEngine;

public class NetworkAnimator : MonoBehaviour
{
    public GameObject obj = null;
    private Animator _animator;

    void Awake()
    {
        if (!obj)
            obj = gameObject;
        if (obj.GetComponent<Animator>() == null)
            return;
        _animator = obj.GetComponent<Animator>();
        _animator.SetLayerWeight(1, 1f);
    }

    public void PlayAnimation(string anim, bool forward = true, int type = 0)
    {
        if (obj.transform.animation != null && obj.transform.animation[anim] != null && obj.transform.animation.IsPlaying(anim))
            return;
        networkView.RPC("StartAnimation", RPCMode.All, anim, forward, type);
    }

    public void StopAnimation()
    {
        networkView.RPC("StopPlaying", RPCMode.All);
    }

    [RPC]
    public void StartAnimation(string anim, bool forward = true, int type = 0)
    {
        if (anim == null || anim.Length <= 0)
            return;
        if (obj.transform.animation != null && obj.transform.animation[anim] != null)
        {
            if (obj.transform.animation.IsPlaying(anim))
                return;
            AnimationState animState = obj.transform.animation[anim];
            //obj.transform.animation.Stop();
            //animState.time = ((forward) ? 0 : animState.length);
            animState.speed = ((forward) ? 1 : -1);
            obj.transform.animation.Play(anim);
            return;
        }
        StartMecanimClip(anim, type);
    }

    [RPC]
    public void StopPlaying()
    {
        obj.transform.animation.Stop();
    }

    private void StartMecanimClip(string anim, int type = 0)
    {
        StartCoroutine(MecanimClip(anim, type));

    }

    IEnumerator MecanimClip(string animBoolName, int type = 0)
    {
        if (_animator)
        {
            _animator.SetBool(animBoolName, true);
            if (type != 0)
                _animator.SetInteger("AttackType", type);
        }
        yield return null;
        yield return null;
        if (_animator)
            _animator.SetBool(animBoolName, false);
    }
}
