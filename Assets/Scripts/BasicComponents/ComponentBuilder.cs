using System.Xml;
using UnityEngine;

public class ComponentBuilder : MonoBehaviour
{
    // ONLY TESTING !!!
    public string xmlFile;

    // Use this for initialization
    void Start()
    {
        // this is ONLY for TESTING !!!!!
        getDatafromXML(xmlFile+".xml");
    }

    /// <summary>
    ///     set all important values from health, damage and speed component from the relevant .xml
    /// </summary>
    /// <param name="owner">GameObject which has health, damage and speed components</param>
    /// <param name="dataPath">dataname.xml</param>
    private void getDatafromXML(string dataPath)
    {
        Health healthComp = GetComponent<Health>();
        Speed speedComp = GetComponent<Speed>();
        Damage damageComp = GetComponent<Damage>();
        XmlDocument document = new XMLReader(dataPath).GetXML();
        XmlElement health = null;
        foreach (XmlElement node in document.GetElementsByTagName("health"))
            health = node;

        if (health != null)
        {
            healthComp.SetMaxHealth(float.Parse(health.GetElementsByTagName("maxHealth")[0].InnerText), true);
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
