using UnityEngine;
using System.Collections;

public class ComponentTest : MonoBehaviour 
{
	private Health health;
	private Speed speed;
	private Damage damage;

	public float testMaxHealth;
	public float testHealth;

	public float testDefaultSpeed;
	public float testMaxStamina;
	public float testStamina;
	public float testStaminaRegenaration;

	// Use this for initialization
	void Start () 
	{
		health = gameObject.GetComponent<Health>();
		speed = gameObject.GetComponent<Speed>();

		health.alive = true;
		health.SetMaxHealth(testMaxHealth, true);

		speed.SetDefaultSpeed(testDefaultSpeed);
		speed.SetMaxStamina(testMaxStamina);
		speed.SetStamina(testStamina);
		speed.SetStaminaRegenaration(testStaminaRegenaration);
	}
	
	// Update is called once per frame
	void Update () 
	{
		health.SetHealthToValue(testHealth);
		testHealth = health.HealthPoints;	
		testStamina = speed.Stamina;
	}
}
