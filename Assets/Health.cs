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
			networkView.RPC("SetMaxHealth", RPCMode.Others);
	}
	[RPC]
	public void SetToMinHealth()
	{
		_healthPoints = _minHealth;
		if (networkView.isMine)
			networkView.RPC("SetMaxHealth", RPCMode.Others);
	}
	[RPC]
	public void SetToMaxHealth()
	{
		_healthPoints = _maxHealth;
		if (networkView.isMine)
			networkView.RPC("SetMaxHealth", RPCMode.Others);
	}
	[RPC]
	public float SetHealthToValue(float healthValue)
	{
		_healthPoints = healthValue;		
		if (networkView.isMine)
			networkView.RPC("SetMaxHealth", RPCMode.Others);
		return _healthPoints;
	}
	[RPC]
	public float IncHealth(float healthValue)
	{
		_healthPoints += healthValue;	
		if (networkView.isMine)
			networkView.RPC("SetMaxHealth", RPCMode.Others);
		return _healthPoints;
	}
	[RPC]
	public float DecHealth(float healthValue)
	{
		_healthPoints -= healthValue;	
		if (networkView.isMine)
			networkView.RPC("SetMaxHealth", RPCMode.Others);
		return _healthPoints;
	}
}
