using System.Xml;
using UnityEngine;

public class Minion : MonoBehaviour
{
    public GameObject minion;

    // Use this for initialization
    void Start()
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
            healthComp.SetMaxHealth(float.Parse(health.GetElementsByTagName("maxHealth")[0].InnerText), false);
            healthComp.SetHealthToValue(float.Parse(health.GetElementsByTagName("maxHealth")[0].InnerText));
            healthComp.SetIncMaxHealth(0.0f);
            healthComp.SetMinHealth(0.0f);
            healthComp.SetMaxHealthMultiplier(1.0f);
            healthComp.keepDeadUnitTime = float.Parse(health.GetElementsByTagName("keepDeadUnitTime")[0].InnerText);
        }
        XmlElement speed = null;
        foreach (XmlElement node in document.GetElementsByTagName("speed"))
            speed = node;

        if (speed != null)
        {
            speedComp.SetDefaultSpeed(float.Parse(speed.GetElementsByTagName("defaultSpeed")[0].InnerText));
            speedComp.SetSprintSpeed(float.Parse(speed.GetElementsByTagName("sprintSpeed")[0].InnerText));
            speedComp.SetMaxStamina(float.Parse(speed.GetElementsByTagName("maxStamina")[0].InnerText));
            speedComp.SetStaminaRegenaration(float.Parse(speed.GetElementsByTagName("staminaRegeneration")[0].InnerText));
            speedComp.SetStaminaDecay(float.Parse(speed.GetElementsByTagName("staminaDecay")[0].InnerText));
            speedComp.SetMinStamina(float.Parse(speed.GetElementsByTagName("minStamina")[0].InnerText));
        }
        XmlElement damage = null;
        foreach (XmlElement node in document.GetElementsByTagName("damage"))
            damage = node;
        if (health != null)
        {
            damageComp.SetDefaultDamage(float.Parse(damage.GetElementsByTagName("defaultDamage")[0].InnerText));
            damageComp.SetHitSpeed(float.Parse(damage.GetElementsByTagName("hitSpeed")[0].InnerText));
        }
    }
}
