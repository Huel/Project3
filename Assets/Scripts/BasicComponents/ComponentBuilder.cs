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
                 characterComp.basicAttack.skillName = skill.GetElementsByTagName("skill0")[0].InnerText;
                 characterComp.skill1.skillName = skill.GetElementsByTagName("skill1")[0].InnerText;
                 characterComp.skill2.skillName = skill.GetElementsByTagName("skill2")[0].InnerText;
                 characterComp.skill3.skillName = skill.GetElementsByTagName("skill3")[0].InnerText;
                 characterComp.skill4.skillName = skill.GetElementsByTagName("skill4")[0].InnerText;
                 characterComp.heroicAura.skillName = skill.GetElementsByTagName("skillAura")[0].InnerText;

                 Debug.Log(skill.GetElementsByTagName("skill0")[0].InnerText + ", " +
                     skill.GetElementsByTagName("skill1")[0].InnerText + ", " +
                     skill.GetElementsByTagName("skill2")[0].InnerText + ", " +
                     skill.GetElementsByTagName("skill3")[0].InnerText + ", " +
                     skill.GetElementsByTagName("skill4")[0].InnerText + ", " +
                     skill.GetElementsByTagName("skillAura")[0].InnerText);
                 Debug.Log(characterComp.basicAttack.skillName + ", " +
                     characterComp.skill1.skillName + ", " +
                     characterComp.skill2.skillName + ", " +
                     characterComp.skill3.skillName + ", " +
                     characterComp.skill4.skillName + ", " +
                     characterComp.heroicAura.skillName);
             }
        }
        state = LoadingState.Loaded;
        enabled = false;
    }
}
