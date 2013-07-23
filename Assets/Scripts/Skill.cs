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
    private string valueType;
    private string target;

	public Modifier(Skill skill, string field, float value, string valueType, string target)
	{
        this.skill = skill;
        type = modifierType.modify;
        this.field = field;
        this.value = value;
        this.valueType = valueType;
        this.target = target;
	}

    public void execute()
    {
        switch(type)
        {
            case modifierType.modify:
				switch(field)
				{
					case "Health":
						Health comp = null;
						if(target == "Self") comp = skill.gameObject.GetComponent<Health>();
						if(target == "Target" && skill.gameObject.GetComponent<Range>().GetNearestTarget() != null) skill.gameObject.GetComponent<Range>().GetNearestTarget().GetComponent<Health>();
						if(comp != null)
						{
							//if(valueType=="increase") comp.IncHealth(skill.gameObject.GetComponent<Damage>().DefaultDamage * value);
							//if(valueType=="decrease") comp.DecHealth(skill.gameObject.GetComponent<Damage>().DefaultDamage * value);
						}
						break;
				}
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
		}
		return true;
	}
}

public enum SkillState { Ready, InExecution, Active, OnCooldown };

public class Skill : MonoBehaviour
{
    private string skillName;

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
        this.skillName = name;

        ConvertXML();

        actualCooldown = 0f;
        actualCastingTime = 0f;
        _enabled = true;
        _state = SkillState.Ready;
		
        //@TODO: Test only
		//Execute();
    }

    public bool Execute()
    {
        if (!_enabled) return false;
        if (_state != SkillState.Ready) return false;
		if (trigger != null && !trigger.check()) return false;
		actualCooldown = cooldown;
		actualCastingTime = castingTime;
        _state = SkillState.InExecution;
        return true;
    }

    void Update()
    {
        if (_state == SkillState.Ready) return;
		
        if (_state == SkillState.InExecution)
        {
            actualCastingTime -= Time.deltaTime;
            if (actualCastingTime <= 0)
                _state = SkillState.Active;
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
            if (actualCooldown <= 0)
                _state = SkillState.Ready;
        }
    }

    private void ConvertXML()
    {
        XmlDocument document = new XMLReader("Skills.xml").GetXML();
		XmlElement skillNode = null;
		foreach(XmlElement node in document.GetElementsByTagName("skill"))
			if(node.GetAttribute("name") == skillName)
				skillNode = node;
		if(skillNode != null)
		{
			XmlNodeList typeList = skillNode.GetElementsByTagName("type");
			XmlNodeList triggerList = skillNode.GetElementsByTagName("trigger");
			XmlNodeList castingTimeList = skillNode.GetElementsByTagName("castingTime");
			XmlNodeList cooldownList = skillNode.GetElementsByTagName("cooldown");
            cooldown = ((cooldownList[0] as XmlElement).HasAttribute("type")) ? gameObject.GetComponent<Damage>().HitSpeed * float.Parse(cooldownList[0].InnerText) : float.Parse(cooldownList[0].InnerText);
            castingTime = ((castingTimeList[0] as XmlElement).HasAttribute("type")) ? gameObject.GetComponent<Damage>().HitSpeed * float.Parse(castingTimeList[0].InnerText) : float.Parse(castingTimeList[0].InnerText);
			
			List<TargetType> compareTypes = new List<TargetType> { TargetType.Hero, TargetType.Minion, TargetType.Spot, TargetType.Valve };
			List<TargetType> targetTypes;
			string[] fieldStrings;
            string field, target, valueType;
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
                        valueType = (skill.ChildNodes[2] as XmlElement).GetAttribute("type");
                        target = skill.ChildNodes[3].InnerText;
                        modifiers.Add(new Modifier(this, field, parsedValue, valueType, target));
						break;
				}
			}
		}
    }
}