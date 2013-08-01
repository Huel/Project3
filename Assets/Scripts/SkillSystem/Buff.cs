using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Xml;

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
        if(!initializated || auraPart) return;
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
        foreach (XmlElement node in document.GetElementsByTagName("skill"))
            if (node.GetAttribute("name") == skill.skillName)
                skillNode = node;

        XmlElement buffNode = null;
        foreach (XmlElement node in skillNode.GetElementsByTagName((isDebuff)?"debuff":"buff"))
            if (node.GetAttribute("id") == buffName)
                buffNode = node;

        XmlNodeList addModifierList = buffNode.GetElementsByTagName("addModifier");
        XmlNodeList removeModifierList = buffNode.GetElementsByTagName("removeModifier");

        List<Modifier> adders = new List<Modifier>();
        foreach (XmlNode addModifier in addModifierList)
            adders.Add(new Modifier(skill.gameObject, gameObject, addModifier.ChildNodes[0].InnerText, addModifier.ChildNodes[1].InnerText, (addModifier.ChildNodes[1] as XmlElement).GetAttribute("type")));

        List<Modifier> removers = new List<Modifier>();
        foreach (XmlNode removeModifier in removeModifierList)
            removers.Add(new Modifier(skill.gameObject, gameObject, removeModifier.ChildNodes[0].InnerText, removeModifier.ChildNodes[1].InnerText, (removeModifier.ChildNodes[1] as XmlElement).GetAttribute("type")));

        buffID = buffName;
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