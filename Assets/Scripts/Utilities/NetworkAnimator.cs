using System.Collections;
using UnityEngine;

public class NetworkAnimator : MonoBehaviour
{

    private Animator _animator;

    void Awake()
    {
        if (GetComponent<Animator>() == null)
            return;
        _animator = GetComponent<Animator>();
        _animator.SetLayerWeight(1, 1f);
        
    }

    public void PlayAnimation(string anim, bool forward = true)
    {
        networkView.RPC("StartAnimation", RPCMode.All, anim, forward);
    }

    [RPC]
    public void StartAnimation(string anim, bool forward = true)
    {
        if (anim == null || anim.Length <= 0)
            return;
        if (transform.animation != null && transform.animation[anim] != null)
        {
            AnimationState animState = transform.animation[anim];
            transform.animation.Stop();
            animState.time = ((forward) ? 0 : animState.length);
            animState.speed = ((forward) ? 1 : -1);
            transform.animation.Play(anim);
            return;
        }
        StartMecanimClip(anim);
    }

    private void StartMecanimClip(string anim)
    { 
        StartCoroutine(MecanimClip(anim));
        
    }

    IEnumerator MecanimClip(string animBoolName)
    {
        
        if (_animator)
            _animator.SetBool(animBoolName, true);
        yield return null;
        yield return null;
        if (_animator)
            _animator.SetBool(animBoolName, false);    
    }
}
