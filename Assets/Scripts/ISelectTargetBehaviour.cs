using UnityEngine;
using System.Collections;

public delegate void OnNewTargetAlert(Target target);

public interface ISelectTargetBehaviour
{
    void SetAttentionRange(Range attentionRange);

    void IncAttention(Target target, float attention);

    void RemoveAttention(Target target);

    void GetTarget();

    void AddListener(OnNewTargetAlert listenerFunction);

    void RemoveListener(OnNewTargetAlert listenerFunction);
}
