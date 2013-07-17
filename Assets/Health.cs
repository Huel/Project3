using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour 
{
	private float _healthPoints;
	private float _maxHealth;
	private float _minHealth;

	private float _deadCounter;

	public float keepDeadUnitTime;

	public bool immortal;
	public bool invulnerable;
	public bool alive;

	public float destroyTimer;
 
	public float HealthPoints
	{
		get { return _healthPoints; }
	}
	public float MaxHealth
	{
		get { return _maxHealth; }
	}
	public float MinHealth
	{
		get { return _minHealth; }
	}
	public bool isAlive()
	{
		return alive;
	}

	[RPC]
	public void SetMaxHealth(float maxHealth, bool incHealth)
	{
		if (maxHealth > _minHealth)
			_maxHealth = maxHealth;
		if (incHealth)
			_healthPoints = _maxHealth;
		if (networkView.isMine)
			networkView.RPC("SetMaxHealth", RPCMode.Others);
	}
	[RPC]
	public void SetMinHealth(float minHealth)
	{
		if (minHealth >= 0)
			_minHealth = minHealth;
		if (networkView.isMine)
			networkView.RPC("SetMinHealth", RPCMode.Others);
	}
	[RPC]
	public void SetToMinHealth()
	{
		_healthPoints = _minHealth;
		if (networkView.isMine)
			networkView.RPC("SetToMinHealth", RPCMode.Others);
	}
	[RPC]
	public void SetToMaxHealth()
	{
		_healthPoints = _maxHealth;
		if (networkView.isMine)
			networkView.RPC("SetToMaxHealth", RPCMode.Others);
	}
	[RPC]
	public float SetHealthToValue(float healthValue)
	{
		_healthPoints = healthValue;		
		if (networkView.isMine)
			networkView.RPC("SetHealthToValue", RPCMode.Others);
		return _healthPoints;
	}
	[RPC]
	public float IncHealth(float healthValue)
	{
		_healthPoints += healthValue;
		if (_healthPoints > _maxHealth)
			_healthPoints = _maxHealth;
		if (networkView.isMine)
			networkView.RPC("IncHealth", RPCMode.Others);
		return _healthPoints;
	}
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
			networkView.RPC("DecHealth", RPCMode.Others);
		return _healthPoints;
	}

	void Update()
	{
		if (_healthPoints > 0 && !alive)
			alive = true;
		else if (_healthPoints <= 0)
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
