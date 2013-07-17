using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour 
{
    private float _defaultDamage;
    private float _incDamage;
    private float _hitSpeed;
    private float _incHitSpeed;
    private float _damageMultiplier;
    private float _hitSpeedMultiplier;

    // properties are read-only
    public float DefaultDamage
    {
        get { return _defaultDamage; }
    }
    public float IncDamage
    {
        get { return _incDamage; }
    }
    public float HitSpeed
    {
        get { return _hitSpeed; }
    }
    public float IncHitSpeed
    {
        get { return _incHitSpeed; }
    }
    public float DamageMultiplier
    {
        get { return _damageMultiplier; }
    }
    public float HitSpeedMultiplierHitRate
    {
        get { return _hitSpeedMultiplier; }
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
    public void SetHitSpeed(float hitSpeed)
    {
        _hitSpeed = hitSpeed;
        if (networkView.isMine)
            networkView.RPC("SetHitSpeed", RPCMode.Others);
    }
    [RPC]
    public void SetIncHitSpeed(float incHitSpeed)
    {
        _incHitSpeed = incHitSpeed;
        if (networkView.isMine)
            networkView.RPC("SetIncHitSpeed", RPCMode.Others);
    }
    [RPC]
    public void SetIncHitSpeedByMultiplier()
    {
        _incHitSpeed *= _hitSpeedMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetIncHitSpeedByMultiplier", RPCMode.Others);
    }
    [RPC]
    public void SetDamageMultiplier(float damageMultiplier)
    {
        _damageMultiplier = damageMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetDamageMultiplier", RPCMode.Others);
    }
    [RPC]
    public void SetHitSpeedMultiplier(float hitSpeedMultiplier)
    {
        _hitSpeedMultiplier = hitSpeedMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetHitSpeedMultiplier", RPCMode.Others);
    }
}
