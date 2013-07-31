using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Modifier
{
	private Skill skill;
	private modifierType type;
	enum modifierType { modify, buff, debuff, buffModification, aura};

	private string field;
	private string value;
    private float baseValue = -1;
	private string valueType;
	private string target;
	private List<Team.TeamIdentifier> targetTeams;

	public Modifier(Skill skill, string field, string value, string valueType, string target, string targetTeam)
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

    private string componentName;
    private string targetTeamString;

    public Modifier(Skill skill, string componentName, string target, string targetTeam, bool isDebuff)
    {
        this.skill = skill;
        type = (isDebuff) ? modifierType.debuff : modifierType.buff;
        this.componentName = componentName;
        this.target = target;
        targetTeamString = targetTeam;

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

    private GameObject sourceObject = null, targetObject = null;

    public Modifier(GameObject source, GameObject target, string field, string value, string valueType)
    {
        type = modifierType.buffModification;
        sourceObject = source;
        targetObject = target;
        this.field = field;
        this.value = value;
        this.valueType = valueType;
    }

    private List<TargetType> targetTypes;
    private string buffName;
    private float minValue, maxValue;

    public Modifier(Skill skill, string buffName, List<TargetType> targetTypes, string targetTeam, string minValue, string maxValue)
    {
        this.skill = skill;
        type = modifierType.aura;
        this.buffName = buffName;
        this.targetTypes = targetTypes;

        this.minValue = float.Parse(minValue);
        this.maxValue = float.Parse(maxValue);

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

	public void Execute()
	{
		switch (type)
		{
			case modifierType.modify:
				Modificate();
				break;

            case modifierType.buff:
                Buff();
                break;

            case modifierType.debuff:
                Buff();
                break;

            case modifierType.buffModification:
                Modificate();
                break;

            case modifierType.aura:
                Aura aura = skill.gameObject.AddComponent<Aura>();
                aura.Init(skill, buffName, targetTypes, targetTeams, minValue, maxValue);
                break;
		}
	}

    public void ChangeValue(float value)
    {
        if (baseValue == -1) baseValue = float.Parse(this.value);
        this.value = (baseValue * value).ToString();
    }

    private GameObject SearchForObject(Skill skill, string targetIdentifier, List<Team.TeamIdentifier> teamIdentifiers)
    {
        GameObject targetObject = null;

        if (targetIdentifier == "Self") targetObject = skill.gameObject;
        if (targetIdentifier == "Target")
        {
            Target contact = skill.gameObject.GetComponent<Combat>().trigger.GetContactByTypesAndTeam(new List<TargetType> { TargetType.Hero, TargetType.Minion }, teamIdentifiers);
            if (contact != null)
                targetObject = contact.gameObject;
        }

        return targetObject;
    }

    private void Modificate()
    {
        if (sourceObject == null)
            sourceObject = skill.gameObject;
        if (targetObject == null)
            targetObject = SearchForObject(skill, target, targetTeams);

        if (targetObject == null) return;

        switch (field)
        {
            case "Health":
                Health comp = targetObject.GetComponent<Health>();
                if (valueType == "setInvulnerable") comp.invulnerable = bool.Parse(value);
                if (valueType == "set") comp.SetHealth(float.Parse(value));
                if (valueType == "heal") comp.IncHealth(comp.MaxHealth * float.Parse(value));
                if (valueType == "increase") comp.IncHealth(sourceObject.GetComponent<Damage>().DefaultDamage * float.Parse(value));
                if (valueType == "decrease")
                {
                    comp.DecHealth(sourceObject.GetComponent<Damage>().DefaultDamage * float.Parse(value));
                    if (targetObject.GetComponent<Target>().type == TargetType.Hero && sourceObject.GetComponent<Target>().type == TargetType.Hero)
                        targetObject.GetComponent<LastHeroDamage>().SetSource(sourceObject.networkView.viewID);
                }
                break;

            case "Speed":
                Speed speedComp = targetObject.GetComponent<Speed>();
                if (valueType == "setSpeedMultiplier") speedComp.SetSpeedMultiplier(float.Parse(value));
                break;
        }
    }

    private void Buff()
    {
        if (targetObject == null)
            targetObject = SearchForObject(skill, target, targetTeams);

        if (targetObject == null) return;

        Debug.Log("Apply Buff " + componentName + " to " + targetObject.ToString() + " from " + skill.gameObject.ToString());
        BuffBehaviour buff = targetObject.AddComponent<BuffBehaviour>();
        buff.Load(skill, componentName, (type == modifierType.debuff));
    }
}
