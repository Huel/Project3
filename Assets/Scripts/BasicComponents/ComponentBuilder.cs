using System.Xml;
using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class ComponentBuilder : MonoBehaviour
{
    public enum LoadingState { InActive, Loaded, NotLoaded }
    public string xmlFile;

    public LoadingState state = LoadingState.InActive;
    public bool healthParsed = false;
    public bool speedParsed = false;
    public bool damageParsed = false;

    void Update()
    {
        if (!NetworkManager.isNetwork)
            return;
        if (networkView.isMine)
        {
            getDatafromXML(xmlFile + ".xml");
        }
        else
        {
            state = LoadingState.NotLoaded;
            enabled = false;
        }
    }

    private void getDatafromXML(string dataPath)
    {
        Health healthComp = GetComponent<Health>();
        Speed speedComp = GetComponent<Speed>();
        Damage damageComp = GetComponent<Damage>();

        XmlDocument document = new XMLReader(dataPath).GetXML();

        XmlElement health = null;
        foreach (XmlElement node in document.GetElementsByTagName("health"))
            health = node;

        if (healthComp != null && health != null)
        {
            healthComp.SetMaxHealth(float.Parse(health.GetElementsByTagName("maxHealth")[0].InnerText), true);
            healthComp.SetHealthRegeneration(float.Parse(health.GetElementsByTagName("regeneration")[0].InnerText));
            healthComp.keepDeadUnitTime = float.Parse(health.GetElementsByTagName("keepDeadUnitTime")[0].InnerText);
            healthParsed = true;
        }

        XmlElement speed = null;
        foreach (XmlElement node in document.GetElementsByTagName("speed"))
            speed = node;

        if (speedComp != null && speed != null)
        {
            speedComp.SetDefaultSpeed(float.Parse(speed.GetElementsByTagName("defaultSpeed")[0].InnerText));
            speedComp.SetSprintSpeed(float.Parse(speed.GetElementsByTagName("sprintSpeed")[0].InnerText));
            speedComp.SetMaxStamina(float.Parse(speed.GetElementsByTagName("maxStamina")[0].InnerText));
            speedComp.SetStaminaRegenaration(float.Parse(speed.GetElementsByTagName("staminaRegeneration")[0].InnerText));
            speedComp.SetStaminaDecay(float.Parse(speed.GetElementsByTagName("staminaDecay")[0].InnerText));
            speedComp.SetMinStamina(float.Parse(speed.GetElementsByTagName("minStamina")[0].InnerText));
            speedParsed = true;
        }

        XmlElement damage = null;
        foreach (XmlElement node in document.GetElementsByTagName("damage"))
            damage = node;
        if (damageComp != null && damage != null)
        {
            damageComp.SetDefaultDamage(float.Parse(damage.GetElementsByTagName("defaultDamage")[0].InnerText));
            damageComp.SetHitSpeed(float.Parse(damage.GetElementsByTagName("hitSpeed")[0].InnerText));
            damageParsed = true;
        }
        state = LoadingState.Loaded;
        enabled = false;
    }
}
