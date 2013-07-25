using UnityEngine;

public class Health : MonoBehaviour
{
    private float _healthPoints = 10f;
    private float _maxHealth = 10f;
    private float _incMaxHealth;
    private float _minHealth = 0f;
    private float _maxHealthMultiplier = 1f;

    private float _deadCounter;

    public float keepDeadUnitTime = 5f;

    public bool immortal;
    public bool invulnerable;
    public bool alive = true;

    public float HealthPoints
    {
        get { return _healthPoints; }
    }
    public float MaxHealth
    {
        get { return (_maxHealth * _maxHealthMultiplier) + _incMaxHealth; }
    }
    public float MinHealth
    {
        get { return _minHealth; }
    }
    public bool isAlive()
    {
        return alive;
    }
    /// <summary>
    ///     set the maximum health
    /// </summary>
    /// <param name="maxHealth">set maxHealth multiply with maxHealthMultiplier</param>
    /// <param name="incHealth">set the health to the max if true</param>
    [RPC]
    public void SetMaxHealth(float maxHealth, bool incHealth)
    {
        _maxHealth = maxHealth;
        if (networkView.isMine)
        {
            networkView.RPC("SetMaxHealth", RPCMode.OthersBuffered, maxHealth, incHealth);
            if (incHealth)
                SetToMaxHealth();
        }

    }
    /// <summary>
    ///     set the minimum health
    /// </summary>
    /// <param name="minHealth">value will never be under 0</param>
    [RPC]
    public void SetMinHealth(float minHealth)
    {
        _minHealth = Mathf.Max(0, minHealth);

        if (networkView.isMine)
        {
            networkView.RPC("SetMinHealth", RPCMode.OthersBuffered, MinHealth);
        }

    }
    /// <summary>
    ///     set the increased maximum health, no maxHealthMultiplier needed
    /// </summary>
    /// <param name="incMaxHealth"></param>
    [RPC]
    public void SetIncreasedMaxHealth(float incMaxHealth)
    {
        _incMaxHealth = incMaxHealth;
        if (networkView.isMine)
        {
            networkView.RPC("SetIncreasedMaxHealth", RPCMode.Others, incMaxHealth);
            IncHealth(incMaxHealth);
        }

    }
    /// <summary>
    ///     set the health to the minimum
    /// </summary>
    public void SetToMinHealth()
    {
        networkView.RPC("SetHealth", networkView.owner, MinHealth);
    }
    /// <summary>
    ///     set the health to the MAX !!
    /// </summary>
    public void SetToMaxHealth()
    {
        networkView.RPC("SetHealth", networkView.owner, MaxHealth);
    }
    /// <summary>
    ///     set and return the health
    /// </summary>
    /// <param name="healthValue">set the health dependet on maxHealth and minHealth</param>
    /// <returns>return the new health</returns>
    [RPC]
    public float SetHealth(float healthValue)
    {
        _healthPoints = Mathf.Clamp(healthValue, MinHealth, MaxHealth);
        if (networkView.isMine)
        {
            networkView.RPC("SetHealth", RPCMode.Others, HealthPoints);
        }
        return HealthPoints;
    }
    /// <summary>
    ///     set the multiplier for health buff
    /// </summary>
    /// <param name="maxHealthMultiplier"></param>
    [RPC]
    public void SetMaxHealthMultiplier(float maxHealthMultiplier)
    {
        float tempMaxHealth = MaxHealth;
        _maxHealthMultiplier = maxHealthMultiplier;
        if (networkView.isMine)
        {
            networkView.RPC("SetMaxHealthMultiplier", RPCMode.Others, maxHealthMultiplier);
            IncHealth(MaxHealth - tempMaxHealth);
        }

    }
    /// <summary>
    ///     increases the current health
    /// </summary>
    /// <param name="healthValue">the value added to current health</param>
    /// <returns>value is never higher than maxHealth</returns>
    public float IncHealth(float healthValue)
    {
        networkView.RPC("SetHealth", networkView.owner, HealthPoints + healthValue);
        return HealthPoints;
    }
    /// <summary>
    ///     decreases the current health
    /// </summary>
    /// <param name="healthValue">the value substract to current health</param>
    /// <returns>value is never lower than minHealth</returns>
    public float DecHealth(float healthValue)
    {
        return IncHealth(-healthValue);
    }

    void Update()
    {
        if (_healthPoints > 0 && !alive)
            alive = true;
        else if (_healthPoints <= 0 && !immortal)
        {
            alive = false;
            gameObject.GetComponent<Target>().type = TargetType.Dead;
        }
        if (!alive)
            _deadCounter += Time.deltaTime;
        else
            _deadCounter = 0;

        if (_deadCounter >= keepDeadUnitTime)
        {
            NetworkView.DestroyObject(gameObject);
        }
    }
}
