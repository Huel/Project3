using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class BuffBehaviour : MonoBehaviour
{
    public string buffID;
    private bool initializated = false;
    private bool auraPart;

    private float duration;
    private List<Modifier> removeModifiers;
    private List<Modifier> addModifiers;

    private void Init(List<Modifier> adds, float duration, List<Modifier> removes)
    {
        this.duration = duration;
        removeModifiers = removes;
        initializated = true;
        foreach (Modifier modifier in adds)
            modifier.Execute();
    }

    private void Init(List<Modifier> adds, List<Modifier> removes)
    {
        addModifiers = adds;
        removeModifiers = removes;
        initializated = true;
        foreach (Modifier modifier in adds)
            modifier.Execute();
    }

    void Update()
    {
        if (!initializated || auraPart) return;
        if (duration > 0)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
                Remove();
        }
    }

    public void ChangeAuraValue(float value)
    {
        foreach (Modifier adder in addModifiers)
        {
            adder.ChangeValue(value);
            adder.Execute();
        }
    }

    public void Remove()
    {
        foreach (Modifier modifier in removeModifiers)
            modifier.Execute();
        Destroy(this);
    }

    public void Load(Skill skill, string buffName, bool isDebuff, bool auraPart = false)
    {
        XmlDocument document = new XMLReader("Skills.xml").GetXML();
        XmlElement skillNode = null;
        foreach (XmlElement node in document.GetElementsByTagName("skill").Cast<XmlElement>().Where(node => node.GetAttribute("name") == skill.skillName))
            skillNode = node;

        buffID = buffName;

        XmlElement buffNode = null;
        foreach (XmlElement node in skillNode.GetElementsByTagName((isDebuff) ? "debuff" : "buff").Cast<XmlElement>().Where(node => node.GetAttribute("id") == buffID))
            buffNode = node;

        if (buffNode.HasAttribute("randomEffect") && buffNode.GetAttribute("randomEffect") == "true")
        {
            auraPart = false;
            XmlNodeList addBuffList = buffNode.GetElementsByTagName("addBuff");
            buffID = addBuffList[UnityEngine.Random.Range(0, addBuffList.Count)].ChildNodes[0].InnerText;
            foreach (XmlElement node in skillNode.GetElementsByTagName((isDebuff) ? "debuff" : "buff"))
                if (node.GetAttribute("id") == buffID)
                    buffNode = node;
        }

        XmlNodeList addModifierList = buffNode.GetElementsByTagName("addModifier");
        XmlNodeList removeModifierList = buffNode.GetElementsByTagName("removeModifier");

        List<Modifier> adders = (from XmlNode addModifier in addModifierList
                                 select (addModifier.ChildNodes[1] as XmlElement).HasAttribute("type")
                                     ? new Modifier(skill, skill.gameObject, gameObject, addModifier.ChildNodes[0].InnerText, addModifier.ChildNodes[1].InnerText,
                                     (addModifier.ChildNodes[1] as XmlElement).GetAttribute("type"))
                                     : new Modifier(skill, skill.gameObject, gameObject,
                                     addModifier.ChildNodes[0].InnerText, addModifier.ChildNodes[1].InnerText)).ToList();

        List<Modifier> removers = (from XmlNode removeModifier in removeModifierList select (removeModifier.ChildNodes[1] as XmlElement).HasAttribute("type") ? new Modifier(skill, skill.gameObject, gameObject, removeModifier.ChildNodes[0].InnerText, removeModifier.ChildNodes[1].InnerText, (removeModifier.ChildNodes[1] as XmlElement).GetAttribute("type")) : new Modifier(skill, skill.gameObject, gameObject, removeModifier.ChildNodes[0].InnerText, removeModifier.ChildNodes[1].InnerText)).ToList();

        this.auraPart = auraPart;

        if (!auraPart)
        {
            XmlNodeList durationList = buffNode.GetElementsByTagName("duration");
            duration = float.Parse(durationList[0].InnerText);
            Init(adders, duration, removers);
        }
        else
            Init(adders, removers);
    }
}