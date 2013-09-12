using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class Damage : MonoBehaviour
{
    private float _defaultDamage = 3f;
    private float _incDamage;
    private float _hitSpeed = 0.5f;        // Hits per second
    private float _incHitSpeed;
    private float _damageMultiplier = 1.0f;
    private float _hitSpeedMultiplier = 1.0f;

    // properties are read-only

    /// <summary>
    ///     returns damage multiply with damage Multiplier
    /// </summary>
    public float CurrentDamage
    {
        get { return (DefaultDamage * _damageMultiplier) + _incDamage; }
    }
    public float DefaultDamage
    {
        get { return _defaultDamage; }
    }

    /// <summary>
    ///     returns hitspeed multiply with hitspeed Multiplier
    /// </summary>
    public float HitSpeed
    {
        get { return (_hitSpeed * _hitSpeedMultiplier) + _incHitSpeed; }
    }


    // setter methods must be executed on all clients from oject-owner

    /// <summary>
    ///     set the default damage multiply with damage multiplier
    /// </summary>
    /// <param name="defaultDamage">default damage never under 0</param>
    [RPC]
    public void SetDefaultDamage(float defaultDamage)
    {
        _defaultDamage = Mathf.Max(0, defaultDamage);

        if (networkView.isMine)
            networkView.RPC("SetDefaultDamage", RPCMode.OthersBuffered, defaultDamage);
    }
    /// <summary>
    ///     set the increased damage
    /// </summary>
    /// <param name="incDamage">increased damage never under 0</param>
    [RPC]
    public void SetIncreasedDamage(float incDamage)
    {
        _incDamage = incDamage;
        if (networkView.isMine)
            networkView.RPC("SetIncreasedDamage", RPCMode.OthersBuffered, incDamage);
    }
    /// <summary>
    ///     set the damage multiplier
    /// </summary>
    /// <param name="damageMultiplier"></param>
    [RPC]
    public void SetDamageMultiplier(float damageMultiplier)
    {
        _damageMultiplier = damageMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetDamageMultiplier", RPCMode.OthersBuffered, damageMultiplier);
    }
    /// <summary>
    ///     set the default hit speed
    /// </summary>
    /// <param name="hitSpeed">hit speed means how fast you hit</param>
    [RPC]
    public void SetHitSpeed(float hitSpeed)
    {
        _hitSpeed = hitSpeed;
        if (networkView.isMine)
            networkView.RPC("SetHitSpeed", RPCMode.OthersBuffered, hitSpeed);
    }
    /// <summary>
    ///     set the increased hit speed
    /// </summary>
    /// <param name="incHitSpeed">hit speed means hits per second</param>
    [RPC]
    public void SetIncreasedHitSpeed(float incHitSpeed)
    {
        _incHitSpeed = incHitSpeed;
        if (networkView.isMine)
            networkView.RPC("SetIncreasedHitSpeed", RPCMode.OthersBuffered, incHitSpeed);
    }
    /// <summary>
    ///     set the hit speed Multiplier
    /// </summary>
    /// <param name="hitSpeedMultiplier"></param>
    [RPC]
    public void SetHitSpeedMultiplier(float hitSpeedMultiplier)
    {
        _hitSpeedMultiplier = hitSpeedMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetHitSpeedMultiplier", RPCMode.OthersBuffered, hitSpeedMultiplier);
    }
}
