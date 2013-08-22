using System.Collections.Generic;
using UnityEngine;

public class Modifier
{
    private Skill skill;
    private modifierType type;
    enum modifierType { modify, buff, debuff, buffModification, aura, minionAgentManupulation, spot };

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

    public Modifier(Skill skill, GameObject source, GameObject target, string field, string value, string valueType)
    {
        this.skill = skill;
        type = modifierType.buffModification;
        sourceObject = source;
        targetObject = target;
        this.field = field;
        this.value = value;
        this.valueType = valueType;
    }

    private string modificator;

    public Modifier(Skill skill, GameObject source, GameObject target, string modificator, string value)
    {
        this.skill = skill;
        type = modifierType.minionAgentManupulation;
        sourceObject = source;
        targetObject = target;
        this.modificator = modificator;
        this.value = value;
    }

    private List<TargetType> targetTypes;
    private string buffName;
    private float minValue, range;
    private UnityEngine.Object aura = null;
    private bool once;

    public Modifier(Skill skill, string buffName, List<TargetType> targetTypes, string targetTeam, string range, string minValue, bool once)
    {
        this.skill = skill;
        type = modifierType.aura;
        this.buffName = buffName;
        this.targetTypes = targetTypes;

        this.range = float.Parse(range);
        this.minValue = float.Parse(minValue);
        this.once = once;

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

    private string spotPrefabName;
    private UnityEngine.Object spot = null;

    public Modifier(Skill skill, string spotPrefabName)
    {
        this.skill = skill;
        type = modifierType.spot;
        this.spotPrefabName = spotPrefabName;
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
                aura = Network.Instantiate(Resources.Load("aurabuff"), skill.gameObject.transform.position, skill.gameObject.transform.rotation, 1);
                (aura as GameObject).transform.parent = skill.gameObject.transform;
                (aura as GameObject).GetComponent<Aura>().Init(skill, buffName, targetTypes, targetTeams, minValue, range, once);
                (aura as GameObject).GetComponent<SphereCollider>().radius = range;
                break;

            case modifierType.minionAgentManupulation:
                if (!targetObject.GetComponent<Target>().IsMinion()) return;
                if (modificator == "AddSquad")
                {
                    skill.gameObject.GetComponent<Squad>().AddSquadMember(targetObject);
                    break;
                }
                if (modificator == "RemoveSquad")
                {
                    skill.gameObject.GetComponent<Squad>().RemoveSquadMember(targetObject);
                    break;
                }
                GameObject aim = SearchForObject(skill, value, new List<Team.TeamIdentifier>());
                targetObject.GetComponent<MinionAgent>().Manipulate(modificator, value, (aim == null) ? null : aim.GetComponent<Target>());
                break;

            case modifierType.spot:
                spot = Network.Instantiate(Resources.Load(spotPrefabName), skill.gameObject.transform.position, skill.gameObject.transform.rotation, 1);
                (spot as GameObject).GetComponent<Team>().ID = skill.gameObject.GetComponent<Team>().ID;
                break;
        }
    }

    public void deactivateAura()
    {
        if (type != modifierType.aura || aura == null) return;
        (aura as GameObject).GetComponent<Aura>().RemoveEffects();
        MonoBehaviour.Destroy(aura);
        aura = null;
    }

    public void deactivateAuraSensitive(string auraName)
    {
        if (type != modifierType.aura || aura == null || (aura as GameObject).GetComponent<Aura>().buffName != auraName) return;
        deactivateAura();
    }

    public void deactivateSpot()
    {
        if (type != modifierType.spot || spot == null) return;
        MonoBehaviour.Destroy(spot);
        spot = null;
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
        if (targetIdentifier == "EnemyBase")
        {
            GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");
            foreach (GameObject baseObject in bases)
                if (baseObject.GetComponent<Team>().ID != this.targetObject.GetComponent<Team>().ID)
                    targetObject = baseObject;

        }
        if (targetIdentifier == "OwnBase")
        {
            GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");
            foreach (GameObject baseObject in bases)
                if (baseObject.GetComponent<Team>().ID == this.targetObject.GetComponent<Team>().ID)
                    targetObject = baseObject;
        }
        if (targetIdentifier == "Destination")
        {
            targetObject = this.targetObject.GetComponent<MinionAgent>().GetDestination().gameObject;
        }
        if (targetIdentifier == "Origin")
        {
            targetObject = this.targetObject.GetComponent<MinionAgent>().GetOrigin().gameObject;
        }

        return targetObject;
    }

    private void Modificate()
    {
        if (sourceObject == null && skill)
            sourceObject = skill.gameObject;
        GameObject _targetObject;
        if (targetObject == null)
            _targetObject = SearchForObject(skill, target, targetTeams);
        else
            _targetObject = targetObject;

        if (_targetObject == null) return;

        switch (field)
        {
            case "Health":
                Health comp = _targetObject.GetComponent<Health>();
                if (valueType == "setInvulnerable") comp.invulnerable = bool.Parse(value);
                if (valueType == "setImmortal") comp.immortal = bool.Parse(value);
                if (valueType == "setHealthMultiplier") comp.SetMaxHealthMultiplier(float.Parse(value));
                if (valueType == "set") comp.SetHealth(float.Parse(value));
                if (valueType == "heal") comp.IncHealth(comp.MaxHealth * float.Parse(value));
                if (valueType == "increase") comp.IncHealth(sourceObject.GetComponent<Damage>().DefaultDamage * float.Parse(value));
                if (valueType == "decrease")
                {
                    comp.DecHealth(sourceObject.GetComponent<Damage>().DefaultDamage * float.Parse(value));
                    if (_targetObject.GetComponent<Target>().type == TargetType.Hero && sourceObject.GetComponent<Target>().type == TargetType.Hero)
                        _targetObject.GetComponent<LastHeroDamage>().SetSource(sourceObject.networkView.viewID);
                }
                break;

            case "Speed":
                Speed speedComp = _targetObject.GetComponent<Speed>();
                if (valueType == "setSpeedMultiplier") speedComp.SetSpeedMultiplier(float.Parse(value));
                if (valueType == "multiplyDefaultSpeed") speedComp.SetDefaultSpeed(speedComp.DefaultSpeed * float.Parse(value));
                if (valueType == "multiplySprintSpeed") speedComp.SetSprintSpeed(speedComp.SprintSpeed * float.Parse(value));
                break;

            case "Damage":
                Damage damageComp = _targetObject.GetComponent<Damage>();
                if (valueType == "setDamageMultiplier") damageComp.SetDamageMultiplier(float.Parse(value));
                break;

            case "Aura":
                if (valueType == "removeAura") skill.DeactivateAura(value);
                break;

            case "Manipulation":
                if (!_targetObject.GetComponent<Target>().IsMinion()) return;
                if (valueType == "AddSquad")
                {
                    skill.gameObject.GetComponent<Squad>().AddSquadMember(_targetObject);
                    break;
                }
                if (valueType == "RemoveSquad")
                {
                    skill.gameObject.GetComponent<Squad>().RemoveSquadMember(_targetObject);
                    break;
                }
                GameObject aim = SearchForObject(skill, value, new List<Team.TeamIdentifier>());
                _targetObject.GetComponent<MinionAgent>().Manipulate(valueType, value, (aim == null) ? null : aim.GetComponent<Target>());
                break;
        }
    }

    private void Buff()
    {
        GameObject _targetObject;
        if (targetObject == null)
            _targetObject = SearchForObject(skill, target, targetTeams);
        else
            _targetObject = targetObject;

        if (_targetObject == null) return;

        BuffBehaviour buff = _targetObject.AddComponent<BuffBehaviour>();
        buff.Load(skill, componentName, (type == modifierType.debuff));
    }
}
