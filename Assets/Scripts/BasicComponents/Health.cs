using UnityEngine;
using System.Collections;

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
	/// <summary>
	///     get the maximum health multiply with maxHealthMultiplier
	/// </summary>
	public float MaxHealth
	{
		get { return _maxHealth * _maxHealthMultiplier; }
	}
	public float IncMaxHealth
	{
		get { return _incMaxHealth; }
	}
	public float MinHealth
	{
		get { return _minHealth; }
	}
	public float MaxHealthMultiplier
	{
		get { return _maxHealthMultiplier; }
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
		if ( (maxHealth * _maxHealthMultiplier) > _minHealth)
			_maxHealth = maxHealth;
		else if ( (maxHealth * _maxHealthMultiplier) < _minHealth)
			_maxHealth = _minHealth;
		
		if (incHealth)
			_healthPoints = _maxHealth * _maxHealthMultiplier;

		if (networkView.isMine)
			networkView.RPC("SetMaxHealth", RPCMode.Others, maxHealth, incHealth);
	}
	/// <summary>
	///     set the minimum health
	/// </summary>
	/// <param name="minHealth">value will never be under 0</param>
	[RPC]
	public void SetMinHealth(float minHealth)
	{
		if (minHealth >= 0)
			_minHealth = minHealth;
		else
			_minHealth = 0;

		if (networkView.isMine)
            networkView.RPC("SetMinHealth", RPCMode.Others, minHealth);
	}
	/// <summary>
	///     set the increased maximum health, no maxHealthMultiplier needed
	/// </summary>
	/// <param name="incMaxHealth"></param>
	[RPC]
	public void SetIncMaxHealth(float incMaxHealth)
	{
		if (_incMaxHealth > _minHealth)
			_incMaxHealth = incMaxHealth;
		else
			_incMaxHealth = _minHealth;

		if (networkView.isMine)
            networkView.RPC("SetIncMaxHealth", RPCMode.Others, incMaxHealth);
	}
	/// <summary>
	///     set the health to the minimum
	/// </summary>
	[RPC]
	public void SetToMinHealth()
	{
		_healthPoints = _minHealth;
		if (networkView.isMine)
			networkView.RPC("SetToMinHealth", RPCMode.Others);
	}
	/// <summary>
	///     set the health to the MAX !!
	/// </summary>
	[RPC]
	public void SetToMaxHealth()
	{
		_healthPoints = _maxHealth;
		if (networkView.isMine)
			networkView.RPC("SetToMaxHealth", RPCMode.Others);
	}
	/// <summary>
	///     set and return the health
	/// </summary>
	/// <param name="healthValue">set the health dependet on maxHealth and minHealth</param>
	/// <returns>return the new health</returns>
	[RPC]
	public float SetHealthToValue(float healthValue)
	{
		if (healthValue > _maxHealth * _maxHealthMultiplier)
			_healthPoints = _maxHealth * _maxHealthMultiplier;
		else if (healthValue < _minHealth)
			_healthPoints = _minHealth;
		else
			_healthPoints = healthValue;

		if (networkView.isMine)
            networkView.RPC("SetHealthToValue", RPCMode.Others, healthValue);
		return _healthPoints;
	}
	/// <summary>
	///     set the multiplier for health buff
	/// </summary>
	/// <param name="maxHealthMultiplier"></param>
	[RPC]
	public void SetMaxHealthMultiplier(float maxHealthMultiplier)
	{
		_maxHealthMultiplier = maxHealthMultiplier;
		if (networkView.isMine)
            networkView.RPC("SetMaxHealthMultiplier", RPCMode.Others, maxHealthMultiplier);
	}
	/// <summary>
	///     increases the current health
	/// </summary>
	/// <param name="healthValue">the value added to current health</param>
	/// <returns>value is never higher than maxHealth</returns>
	[RPC]
	public float IncHealth(float healthValue)
	{
		_healthPoints += healthValue;
		if (_healthPoints > _maxHealth)
			_healthPoints = _maxHealth;
		if (networkView.isMine)
            networkView.RPC("IncHealth", RPCMode.Others, healthValue);
		return _healthPoints;
	}
	/// <summary>
	///     decreases the current health
	/// </summary>
	/// <param name="healthValue">the value substract to current health</param>
	/// <returns>value is never lower than minHealth</returns>
	[RPC]
	public float DecHealth(float healthValue)
	{
		if (!invulnerable)
			_healthPoints -= healthValue;
		if (_healthPoints < _minHealth)
			_healthPoints = _minHealth;
		if (_minHealth == 0 && _healthPoints < _minHealth)
			if (immortal)
				_healthPoints = 1;
			else
				alive = false;
		if (networkView.isMine)
            networkView.RPC("DecHealth", RPCMode.Others, healthValue);
		return _healthPoints;
	}

	void Update()
	{
		if (_healthPoints > 0 && !alive)
			alive = true;
		else if (_healthPoints <= 0 && !immortal)
			alive = false;
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
