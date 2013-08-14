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
    /// <summary>
    ///     gets information from xml file for Hero and Minion
    /// </summary>
    /// <param name="dataPath">the name of the xml file without ending (.xml)</param>
    private void getDatafromXML(string dataPath)
    {
        Health healthComp = GetComponent<Health>();
        Speed speedComp = GetComponent<Speed>();
        Damage damageComp = GetComponent<Damage>();
        CharacterControllerLogic characterComp = GetComponent<CharacterControllerLogic>();

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
        if (xmlFile == "Minion")
            GetComponent<MinionAgent>().productivity = float.Parse(document.GetElementsByTagName("productivity")[0].InnerText);
        if (xmlFile == "Hero01")
        {
            XmlElement skill = null;
            foreach (XmlElement node in document.GetElementsByTagName("skills"))
                skill = node;
             if (characterComp != null && skill != null)
             {
                 characterComp.basicAttack.name = skill.GetElementsByTagName("skill0")[0].InnerText;
                 characterComp.skill1.name = skill.GetElementsByTagName("skill1")[0].InnerText;
                 characterComp.skill2.name = skill.GetElementsByTagName("skill2")[0].InnerText;
                 characterComp.skill3.name = skill.GetElementsByTagName("skill3")[0].InnerText;
                 characterComp.skill4.name = skill.GetElementsByTagName("skill4")[0].InnerText;
                 characterComp.heroicAura.name = skill.GetElementsByTagName("skillAura")[0].InnerText;

                 Debug.Log(skill.GetElementsByTagName("skill0")[0].InnerText + ", " +
                     skill.GetElementsByTagName("skill1")[0].InnerText + ", " +
                     skill.GetElementsByTagName("skill2")[0].InnerText + ", " +
                     skill.GetElementsByTagName("skill3")[0].InnerText + ", " +
                     skill.GetElementsByTagName("skill4")[0].InnerText + ", " +
                     skill.GetElementsByTagName("skillAura")[0].InnerText);
                 Debug.Log(characterComp.basicAttack.name + ", " +
                     characterComp.skill1.name + ", " +
                     characterComp.skill2.name + ", " +
                     characterComp.skill3.name + ", " +
                     characterComp.skill4.name + ", " +
                     characterComp.heroicAura.name);
             }
        }
        state = LoadingState.Loaded;
        enabled = false;
    }
}
