using UnityEngine;
using System.Collections;
using System.Xml;
using System.Text;
using System.Collections.Generic;

class Modifier
{
	private Skill skill;
    private modifierType type;
    enum modifierType { modify };

    private string field;
    private float value;
    private string target;

	public Modifier(Skill skill, string field, float value, string target)
	{
        this.skill = skill;
        type = modifierType.modify;
        this.field = field;
        this.value = value;
        this.target = target;
	}

    public void execute()
    {
        switch(type)
        {
            case modifierType.modify:
                Debug.Log(field + "   " + value + "   " + target);
                if (field == "Health" && target == "Target" && skill.gameObject.GetComponent<Range>().GetNearestTarget() != null)
                    skill.gameObject.GetComponent<Range>().GetNearestTarget().gameObject.GetComponent<Health>().DecHealth(skill.gameObject.GetComponent<Damage>().DefaultDamage * value);
                if (field == "Health" && target == "Self" && skill.gameObject.GetComponent<Range>().GetNearestTarget() != null)
                    skill.gameObject.GetComponent<Health>().DecHealth(skill.gameObject.GetComponent<Damage>().DefaultDamage * value);
                break;
        }
    }
}

class Trigger
{
	private Skill skill;
	private triggerType type;
	enum triggerType { onRange, onContact, atPosition, instant };
	
	private float radius;
	
	public Trigger (Skill skill, float radius)
	{
		this.skill = skill;
		type = triggerType.onRange;
		this.radius = radius;
	}
	
	List<TargetType> targetTypes;

    public Trigger(Skill skill, List<TargetType> targetTypes)
	{
		this.skill = skill;
		type = triggerType.onContact;
		this.targetTypes = targetTypes;
	}
	
	Vector3 position;

    public Trigger(Skill skill, Vector3 position, float radius)
	{
		this.skill = skill;
		type = triggerType.atPosition;
		this.position = position;
		this.radius = radius;
	}

    public Trigger(Skill skill)
	{
		this.skill = skill;
		type = triggerType.instant;
	}
	
	public bool check()
	{
		switch(type)
		{
			case triggerType.onContact:
				return (skill.gameObject.GetComponent<Range>().GetNearestTargetByTypes(targetTypes) != null);
			case triggerType.atPosition:
				// @TODO: Anpassen...
				return true;
			case triggerType.onRange:
				// @TODO: Anpassen...
				return true;
		}
		return true;
	}
}

public enum SkillState { Ready, InExecution, Active, OnCooldown };

public class Skill : MonoBehaviour
{
    private string name;

    private List<Modifier> modifiers = new List<Modifier>();
    private Trigger trigger = null;
    private float cooldown;
    private float castingTime;

    private float actualCooldown;
    private float actualCastingTime;
    private bool _enabled;
    private SkillState _state;

    bool Enabled { get { return _enabled; } set { _enabled = value; } }
    SkillState State { get { return _state; } }

    public void Init(string name)
    {
        this.name = name;

        ConvertXML();

        actualCooldown = 0f;
        actualCastingTime = 0f;
        _enabled = true;
        _state = SkillState.Ready;
		
        //@TODO: Test only
		Execute();
    }

    public bool Execute()
    {
        if (!_enabled) return false;
        if (_state != SkillState.Ready) return false;
		if (trigger != null && !trigger.check()) return false;
        _state = SkillState.InExecution;
        return true;
    }

    void Update()
    {
        if (_state == SkillState.Ready) return;

        if (_state == SkillState.InExecution)
        {
            actualCastingTime -= Time.deltaTime;
            if (actualCastingTime < castingTime)
            {
                actualCastingTime = castingTime;
                _state = SkillState.Active;
            }
        }

        if (_state == SkillState.Active)
        {
            foreach (Modifier modifier in modifiers)
                modifier.execute();
            _state = SkillState.OnCooldown;
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
			
			List<TargetType> compareTypes = new List<TargetType> { TargetType.Hero, TargetType.Minion, TargetType.Spot, TargetType.Valve };
			List<TargetType> targetTypes;
			string[] fieldStrings;
            string field, target;
			Vector3 position;
			float parsedValue;
			
			string triggerType = triggerList[0].FirstChild.Value;
			triggerType = triggerType.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
			switch(triggerType)
			{
				case "inRange":
					parsedValue = float.Parse(triggerList[0].ChildNodes[1].InnerText);
					trigger = new Trigger(this, parsedValue);
					break;
				
				case "onContact":
                    fieldStrings = triggerList[0].ChildNodes[1].InnerText.Split(new string[] { ", "}, System.StringSplitOptions.None); 
					targetTypes = new List<TargetType>();
					foreach (string type in fieldStrings)
						foreach (TargetType compareType in compareTypes)
							if(compareType.ToString() == type)
								targetTypes.Add(compareType);
					trigger = new Trigger(this, targetTypes);
					break;
				
				case "atPosition":
					fieldStrings = triggerList[0].ChildNodes[1].InnerText.Split(new string[] { ", "}, System.StringSplitOptions.None); 
					position = new Vector3(float.Parse(fieldStrings[0]), float.Parse(fieldStrings[1]), float.Parse(fieldStrings[2]));
					parsedValue = float.Parse(triggerList[0].ChildNodes[2].InnerText);
					trigger = new Trigger(this, position, parsedValue);
					break;
				
				case "instant":
					trigger = new Trigger(this);
					break;
			}
			
			string skillType;
			foreach (XmlNode skill in typeList)
			{
				skillType = skill.FirstChild.Value;
				skillType = skillType.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
				switch(skillType)
				{
					case "modify":
						field = skill.ChildNodes[1].InnerText;
						parsedValue = float.Parse(skill.ChildNodes[2].InnerText);
                        target = skill.ChildNodes[3].InnerText;
                        modifiers.Add(new Modifier(this, field, parsedValue, target));
						break;
				}
			}
		}
    }
}