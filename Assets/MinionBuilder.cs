using UnityEngine;
using System.Collections;

public class MinionBuilder : MonoBehaviour
{
	private Health health;
	private Damage damage;
	private Speed speed;

	public float lifePoints;
	public float maxHealth;
	public float maxHealthMultiplier;

	public float defaultDamage;
	public float hitSpeed;

	public float defaultSpeed;
	
	// Use this for initialization
	void Start ()
	{
		health = gameObject.GetComponent<Health>();
		damage = gameObject.GetComponent<Damage>();
		speed = gameObject.GetComponent<Speed>();

		health.SetMaxHealthMultiplier(maxHealthMultiplier);
		
		health.SetMaxHealth(maxHealth, false);
		health.SetHealthToValue(lifePoints);
		

		damage.SetDefaultDamage(defaultDamage);
		damage.SetHitSpeed(hitSpeed);

		speed.SetDefaultSpeed(defaultSpeed);
	}
	
	// Update is called once per frame
	void Update ()
	{
		lifePoints = health.HealthPoints;
		maxHealth = health.MaxHealth;
	}
}
