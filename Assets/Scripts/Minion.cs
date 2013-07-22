using UnityEngine;
using System.Collections;
using System.Xml;

public class Minion : MonoBehaviour
{
	public GameObject minion;
	
	// Use this for initialization
	void Start () 
	{
		getDatafromXML(minion);
	}

	private void getDatafromXML(GameObject minion)
	{
		Health healthComp = minion.GetComponent<Health>();
		Speed speedComp = minion.GetComponent<Speed>();
		Damage damageComp = minion.GetComponent<Damage>();
		XmlDocument document = new XMLReader("Minion.xml").GetXML();
		XmlElement health = null;
		foreach (XmlElement node in document.GetElementsByTagName("health"))
			health = node;

		if (health != null)
		{ 
			healthComp.SetHealthToValue(float.Parse(health.GetElementsByTagName("healthPoints")[0].InnerText));
			healthComp.SetMaxHealth(float.Parse(health.GetElementsByTagName("maxHealth")[0].InnerText), false);
			healthComp.SetIncMaxHealth(float.Parse(health.GetElementsByTagName("incHealth")[0].InnerText));
			healthComp.SetMinHealth(float.Parse(health.GetElementsByTagName("minHealth")[0].InnerText));
			healthComp.SetMaxHealthMultiplier(float.Parse(health.GetElementsByTagName("maxHealthMultiplier")[0].InnerText));
			healthComp.keepDeadUnitTime = float.Parse(health.GetElementsByTagName("keepDeadUnitTime")[0].InnerText);
		}
		XmlElement speed = null;
		foreach (XmlElement node in document.GetElementsByTagName("speed"))
			speed = node;

		if (speed != null)
		{
			speedComp.SetDefaultSpeed(float.Parse(speed.GetElementsByTagName("defaultSpeed")[0].InnerText));
			speedComp.SetSprintSpeed(float.Parse(speed.GetElementsByTagName("sprintSpeed")[0].InnerText));
			speedComp.SetSpeedMultiplier(float.Parse(speed.GetElementsByTagName("speedMultiplier")[0].InnerText));
		}
		XmlElement damage = null;
		foreach (XmlElement node in document.GetElementsByTagName("damage"))
			damage = node;
		if (health != null)
		{
			damageComp.SetDefaultDamage(float.Parse(damage.GetElementsByTagName("defaultDamage")[0].InnerText));
			damageComp.SetIncDamage(float.Parse(damage.GetElementsByTagName("incDamage")[0].InnerText));
			damageComp.SetHitSpeed(float.Parse(damage.GetElementsByTagName("hitSpeed")[0].InnerText));
			damageComp.SetIncHitSpeed(float.Parse(damage.GetElementsByTagName("incHitSpeed")[0].InnerText));
			damageComp.SetDamageMultiplier(float.Parse(damage.GetElementsByTagName("damageMultiplier")[0].InnerText));
			damageComp.SetHitSpeedMultiplier(float.Parse(damage.GetElementsByTagName("hitSpeedMultiplier")[0].InnerText));
		}
	}
}
