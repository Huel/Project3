using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour 
{
    private float _defaultDamage;
    private float _incDamage;
    private float _hitRate;
    private float _incHitRate;
    private float _damageMultiplier;
    private float _hitRateMultiplier;

    // properties are read-only
    public float DefaultDamage
    {
        get { return _defaultDamage; }
    }
    public float IncDamage
    {
        get { return _incDamage; }
    }
    public float HitRate
    {
        get { return _hitRate; }
    }
    public float IncHitRate
    {
        get { return _incHitRate; }
    }
    public float DamageMultiplier
    {
        get { return _damageMultiplier; }
    }
    public float HitRateMultiplierHitRate
    {
        get { return _hitRateMultiplier; }
    }

    // setter methods must be executed on all clients from oject-owner
    [RPC]
    public void SetDefaultDamage(float defaultDamage)
    {
        _defaultDamage = defaultDamage;
        if (networkView.isMine)
            networkView.RPC("SetDefaultDamage", RPCMode.Others);
    }
    [RPC]
    public void SetIncDamage(float incDamage)
    {
        _incDamage = incDamage;
        if (networkView.isMine)
            networkView.RPC("SetIncDamage", RPCMode.Others);
    }
    [RPC]
    public void SetIncDamageByMultiplier()
    {
        _incDamage *=_damageMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetIncDamageByMultiplier", RPCMode.Others);
    }
    [RPC]
    public void SetHitRate(float hitRate)
    {
        _hitRate = hitRate;
        if (networkView.isMine)
            networkView.RPC("SetHitRate", RPCMode.Others);
    }
    [RPC]
    public void SetIncHitRate(float incHitRate)
    {
        _incHitRate = incHitRate;
        if (networkView.isMine)
            networkView.RPC("SetIncHitRate", RPCMode.Others);
    }
    [RPC]
    public void SetIncHitRateByMultiplier()
    {
        _incHitRate *= _hitRateMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetIncHitRateByMultiplier", RPCMode.Others);
    }
    [RPC]
    public void SetDamageMultiplier(float damageMultiplier)
    {
        _damageMultiplier = damageMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetDamageMultiplier", RPCMode.Others);
    }
    [RPC]
    public void SetHitRateMultiplier(float hitRateMultiplier)
    {
        _hitRateMultiplier = hitRateMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetHitRateMultiplier", RPCMode.Others);
    }
}
