using UnityEngine;
using System.Collections;
using System.Xml;
using System.Text;
using System.Collections.Generic;

class Modifier
{
}

class Trigger
{
}

public enum SkillState { Ready, InExecution, Active, OnCooldown };

public class Skill : MonoBehaviour
{
    private string name;

    private Modifier[] modifiers;
    private Trigger trigger;
    private float cooldown;
    private float castingTime;

    private GameObject owner;
    private float actualCooldown;
    private float actualCastingTime;
    private bool _enabled;
    private SkillState _state;

    bool Enabled { get { return _enabled; } set { _enabled = value; } }
    SkillState State { get { return _state; } }

    public void Init(string name, GameObject owner)
    {
        this.name = name;

        ConvertXML();

        this.owner = owner;
        actualCooldown = 0f;
        actualCastingTime = 0f;
        _enabled = true;
        _state = SkillState.Ready;
    }

    public bool Execute()
    {
        if (!_enabled) return false;
        if (_state != SkillState.Ready) return false;
        _state = SkillState.InExecution;
        return true;
    }

    void Update()
    {
        if (_state == SkillState.Ready || _state == SkillState.Active) return;

        if (_state == SkillState.InExecution)
        {
            actualCastingTime -= Time.deltaTime;
            if (actualCastingTime < castingTime)
            {
                actualCastingTime = castingTime;
                _state = SkillState.Active;
            }
        }

        if (_state == SkillState.OnCooldown)
        {
            actualCooldown -= Time.deltaTime;
            if (actualCooldown < cooldown)
            {
                actualCooldown = cooldown;
                _state = SkillState.Ready;
            }
        }
    }

    private void ConvertXML()
    {
        XmlDocument document = new XMLReader("Skills.xml").GetXML();
		XmlElement skillNode = null;
		foreach(XmlElement node in document.GetElementsByTagName("skill"))
			if(node.GetAttribute("name") == name)
				skillNode = node;
		if(skillNode != null)
		{
			XmlNodeList typeList = skillNode.GetElementsByTagName("type");
			XmlNodeList triggerList = skillNode.GetElementsByTagName("trigger");
			XmlNodeList castingTimeList = skillNode.GetElementsByTagName("castingTime");
			XmlNodeList cooldownList = skillNode.GetElementsByTagName("cooldown");
			cooldown = float.Parse(cooldownList[0].InnerText);
			castingTime = float.Parse(castingTimeList[0].InnerText);
			string triggerType = triggerList[0].FirstChild.Value;
			triggerType = triggerType.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
			switch(triggerType)
			{
				case "inRange":
					float radius = float.Parse(triggerList[0].ChildNodes[1].InnerText);
					Debug.Log("range: " + radius);
					break;
				case "onContact":
                    string[] types = triggerList[0].ChildNodes[1].InnerText.Split(new string[] { ", "}, System.StringSplitOptions.None); 
					List<TargetType> targetTypes = new List<TargetType>();
					foreach (string type in types)
						targetTypes.Add();
					Debug.Log("contact");
					break;
				case "atPosition":
					Debug.Log("position");
					break;
				case "instant":
					Debug.Log("instant");
					break;
			}
		}
		Debug.Log("test");
    }
}