using UnityEngine;

public class Damage : MonoBehaviour
{
    private float _defaultDamage = 3f;
    private float _incDamage;
    private float _hitSpeed;        // Hits per second
<<<<<<< HEAD:Assets/Scripts/BasicComponents/Damage.cs
    private float _incHitSpeed;
    private float _damageMultiplier = 1f;
    private float _hitSpeedMultiplier;
=======
    private float _incHitSpeed;     
    private float _damageMultiplier = 1.0f;
    private float _hitSpeedMultiplier = 1.0f;
>>>>>>> refs/heads/feature/minion&hero_xml:Assets/Scripts/Components/Damage.cs

    // properties are read-only

    /// <summary>
    ///     returns damage multiply with damage Multiplier
    /// </summary>
    public float CurrentDamage
    {
        get { return _defaultDamage * _damageMultiplier; }
    }
    public float DefaultDamage
    {
        get { return _defaultDamage; }
    }
    public float IncDamage
    {
        get { return _incDamage; }
    }
    /// <summary>
    ///     returns hitspeed multiply with hitspeed Multiplier
    /// </summary>
    public float HitSpeed
    {
        get { return _hitSpeed * _hitSpeedMultiplier; }
    }
    public float DeafultHitSpeed
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

    /// <summary>
    ///     set the default damage multiply with damage multiplier
    /// </summary>
    /// <param name="defaultDamage">default damage never under 0</param>
    [RPC]
    public void SetDefaultDamage(float defaultDamage)
    {
        if (_defaultDamage > 0)
            _defaultDamage = defaultDamage * _damageMultiplier;
        else
            _defaultDamage = 0;

        if (networkView.isMine)
            networkView.RPC("SetDefaultDamage", RPCMode.Others, defaultDamage);
    }
    /// <summary>
    ///     set the increased damage
    /// </summary>
    /// <param name="incDamage">increased damage never under 0</param>
    [RPC]
    public void SetIncDamage(float incDamage)
    {
        if (_incDamage > 0)
            _incDamage = incDamage;
        else
            _incDamage = 0;

        if (networkView.isMine)
            networkView.RPC("SetIncDamage", RPCMode.Others, incDamage);
    }
    /// <summary>
    ///     set the increased damage by multiply with the default damage
    /// </summary>
    [RPC]
    public void SetIncDamageByMultiplier()
    {
        _incDamage *= _damageMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetIncDamageByMultiplier", RPCMode.Others);
    }
    /// <summary>
    ///     set the default hit speed
    /// </summary>
    /// <param name="hitSpeed">hit speed means hits per second</param>
    [RPC]
    public void SetHitSpeed(float hitSpeed)
    {
        _hitSpeed = hitSpeed;
        if (networkView.isMine)
            networkView.RPC("SetHitSpeed", RPCMode.Others, hitSpeed);
    }
    /// <summary>
    ///     set the increased hit speed
    /// </summary>
    /// <param name="incHitSpeed">hit speed means hits per second</param>
    [RPC]
    public void SetIncHitSpeed(float incHitSpeed)
    {
        _incHitSpeed = incHitSpeed;
        if (networkView.isMine)
            networkView.RPC("SetIncHitSpeed", RPCMode.Others, incHitSpeed);
    }
    /// <summary>
    ///     set the increased hit speed by multiply with the default hit speed
    /// </summary>
    [RPC]
    public void SetIncHitSpeedByMultiplier()
    {
        _incHitSpeed *= _hitSpeedMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetIncHitSpeedByMultiplier", RPCMode.Others);
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
            networkView.RPC("SetDamageMultiplier", RPCMode.Others, damageMultiplier);
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
            networkView.RPC("SetHitSpeedMultiplier", RPCMode.Others, hitSpeedMultiplier);
    }
}
