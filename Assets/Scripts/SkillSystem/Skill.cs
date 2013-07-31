using System.Collections.Generic;
using System.Xml;
using UnityEngine;

class Modifier
{
	private Skill skill;
	private modifierType type;
	enum modifierType { modify };

	private string field;
	private float value;
	private string valueType;
	private string target;

	private List<Team.TeamIdentifier> targetTeams;

	public Modifier(Skill skill, string field, float value, string valueType, string target, string targetTeam)
	{
		this.skill = skill;
		type = modifierType.modify;
		this.field = field;
		this.value = value;
		this.valueType = valueType;
		this.target = target;

		targetTeams = new List<Team.TeamIdentifier>();
		switch (targetTeam)
		{
			case "Ally":
				targetTeams.Add(skill.gameObject.GetComponent<Team>().ID);
				break;
			case "Enemy":
				targetTeams.Add(skill.gameObject.GetComponent<Team>().Other());
				break;
			case "All":
				targetTeams.Add(skill.gameObject.GetComponent<Team>().ID);
				targetTeams.Add(skill.gameObject.GetComponent<Team>().Other());
				break;
		}
	}

	public void execute()
	{
		switch (type)
		{
			case modifierType.modify:
				switch (field)
				{
					case "Health":
						Health comp = null;
						if (target == "Self") comp = skill.gameObject.GetComponent<Health>();
						if (target == "Target")
						{
							Target contact = skill.gameObject.GetComponent<Combat>().trigger.GetContactByTypesAndTeam(new List<TargetType> { TargetType.Hero, TargetType.Minion }, targetTeams);
							if (contact != null)
								comp = contact.gameObject.GetComponent<Health>();
						}
						if (comp != null)
						{
                            if (valueType == "set") comp.SetHealth(value);
                            if (valueType == "heal") comp.IncHealth(comp.MaxHealth * value);
							if (valueType == "increase") comp.IncHealth(skill.gameObject.GetComponent<Damage>().DefaultDamage * value);
							if (valueType == "decrease")
							{
								comp.DecHealth(skill.gameObject.GetComponent<Damage>().DefaultDamage * value);
								if (comp.gameObject.GetComponent<Target>().type == TargetType.Hero && skill.gameObject.GetComponent<Target>().type == TargetType.Hero)
									comp.gameObject.GetComponent<LastHeroDamage>().SetSource(skill.gameObject.networkView.viewID);
							}
							Debug.Log(skill.gameObject.ToString() + " has executed " + skill.skillName + " to " + comp.gameObject.ToString() + " with " + type.ToString() + "/" + field.ToString() + "/" + target.ToString() + "/" + valueType.ToString() + "/" + value.ToString());
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
	enum triggerType { onContact, instant };

	private float radius;

	private List<TargetType> targetTypes;
    private List<Team.TeamIdentifier> targetTeams;

    public Trigger(Skill skill, List<TargetType> targetTypes, string targetTeam)
	{
		this.skill = skill;
		type = triggerType.onContact;
		this.targetTypes = targetTypes;

        targetTeams = new List<Team.TeamIdentifier>();
        switch (targetTeam)
        {
            case "Ally":
                targetTeams.Add(skill.gameObject.GetComponent<Team>().ID);
                break;
            case "Enemy":
                targetTeams.Add(skill.gameObject.GetComponent<Team>().Other());
                break;
            case "All":
                targetTeams.Add(skill.gameObject.GetComponent<Team>().ID);
                targetTeams.Add(skill.gameObject.GetComponent<Team>().Other());
                break;
        }
	}

	public Trigger(Skill skill)
	{
		this.skill = skill;
		type = triggerType.instant;
	}

	public bool check()
	{
		if(type == triggerType.onContact)
			return (skill.gameObject.GetComponent<Combat>().trigger.GetContactByTypesAndTeam(targetTypes, targetTeams) != null);
		return true;
	}
}

public enum SkillState { Ready, InExecution, Active, OnCooldown };

public class Skill : MonoBehaviour
{
	public string skillName;

	private List<Modifier> modifiers = new List<Modifier>();
	private Trigger trigger = null;
	private float cooldown;
	private float castingTime;

	public float actualCooldown;
	private float actualCastingTime;
	private bool _enabled;
	private SkillState _state;

	bool Enabled { get { return _enabled; } set { _enabled = value; } }
	SkillState State { get { return _state; } }

	public void Start()
	{


		ConvertXML();

		actualCooldown = 0f;
		actualCastingTime = 0f;
		_enabled = true;
		_state = SkillState.Ready;
	}

	public bool Execute()
	{
		if (gameObject == null) return false;
		if (!_enabled) return false;
		if (_state != SkillState.Ready) return false;
		if (trigger != null && !trigger.check()) return false;
		actualCooldown = cooldown;
		actualCastingTime = castingTime;
		_state = SkillState.InExecution;
		gameObject.GetComponent<DebugChangeColor>().Attack();
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
		foreach (XmlElement node in document.GetElementsByTagName("skill"))
			if (node.GetAttribute("name") == skillName)
				skillNode = node;
		if (skillNode != null)
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
			string field, target, valueType, targetType;
			float parsedValue;

			string triggerType = triggerList[0].FirstChild.Value;
			triggerType = triggerType.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
			switch (triggerType)
			{
				case "onContact":
					fieldStrings = triggerList[0].ChildNodes[1].InnerText.Split(new string[] { ", " }, System.StringSplitOptions.None);
					targetTypes = new List<TargetType>();
					foreach (string type in fieldStrings)
						foreach (TargetType compareType in compareTypes)
							if (compareType.ToString() == type)
								targetTypes.Add(compareType);
                    targetType = triggerList[0].ChildNodes[2].InnerText;
					trigger = new Trigger(this, targetTypes, targetType);
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
				switch (skillType)
				{
					case "modify":
						field = skill.ChildNodes[1].InnerText;
						parsedValue = float.Parse(skill.ChildNodes[2].InnerText);
						valueType = (skill.ChildNodes[2] as XmlElement).GetAttribute("type");
						target = skill.ChildNodes[3].InnerText;
						targetType = skill.ChildNodes[4].InnerText;
						modifiers.Add(new Modifier(this, field, parsedValue, valueType, target, targetType));
						break;
				}
			}
		}
	}
}