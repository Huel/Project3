using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour 
{
	private float _healthPoints;
	private float _maxHealth;
	private float _minHealth;

	public float keepDeadUnitTime;

	public bool immortal;
	public bool save;
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

	public void SetMaxHealth(float maxHealth, bool incHealth)
	{
		_maxHealth = maxHealth;
		if (incHealth)
			_healthPoints = _maxHealth;
	}

	public void SetMinHealth(float minHealth)
	{
		if (minHealth >= 0)
			_minHealth = minHealth;
	}

	public void SetToMinHealth()
	{
		_healthPoints = _minHealth;
	}

	public void SetToMaxHealth()
	{
		_healthPoints = _maxHealth;
	}

	public float SetHealthToValue(float healthValue)
	{
		_healthPoints = healthValue;
		return _healthPoints;
	}

	public float IncHealth(float healthValue)
	{
		_healthPoints += healthValue;
		return _healthPoints;
	}

	public float DecHealth(float healthValue)
	{
		_healthPoints -= healthValue;
		return _healthPoints;
	}
}
