using System.Collections.Generic;
using System.Xml;
using UnityEngine;

class SkillTrigger
{
    private Skill skill;
    private triggerType type;
    enum triggerType { onContact, instant };

    private float radius;

    private List<TargetType> targetTypes;
    private List<Team.TeamIdentifier> targetTeams;

    public SkillTrigger(Skill skill, List<TargetType> targetTypes, string targetTeam)
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

    public SkillTrigger(Skill skill)
    {
        this.skill = skill;
        type = triggerType.instant;
    }

    public bool check()
    {
        if (type == triggerType.onContact)
            return (skill.gameObject.GetComponent<Combat>().trigger.GetContactByTypesAndTeam(targetTypes, targetTeams) != null);
        return true;
    }
}

public enum SkillState { Ready, InExecution, Active, OnCooldown, Channeling };

public class Skill : MonoBehaviour
{
    public string skillName;

    private List<Modifier> modifiers = new List<Modifier>();
    private SkillTrigger _skillTrigger = null;
    public float cooldown;
    private float castingTime;

    public float actualCooldown;
    private float actualCastingTime;
    private bool _enabled;
    private SkillState _state;

    private bool isAura;
    private bool isSpot;
    private bool isPassive;

    private NetworkAnimator networkAnimator;

    bool Enabled { get { return _enabled; } set { _enabled = value; } }
    public SkillState State { get { return _state; } }

    public void Start()
    {
        isAura = false;
        isSpot = false;
        isPassive = false;

        ConvertXML();

        networkAnimator = GetComponent<NetworkAnimator>();

        actualCooldown = 0f;
        actualCastingTime = 0f;
        _enabled = true;
        _state = SkillState.Ready;

        if (isPassive)
        {
            Execute();
        }
    }

    public bool Execute()
    {
        if (gameObject == null) return false;
        if (!GetComponent<Health>().IsAlive()) return false;
        if (!_enabled) return false;
        if (isAura && _state == SkillState.Channeling) { foreach (Modifier aura in modifiers) aura.deactivateAura(); _state = SkillState.OnCooldown; return true; }
        if (isSpot && _state == SkillState.Channeling) { foreach (Modifier spot in modifiers) spot.deactivateSpot(); _state = SkillState.OnCooldown; return true; }
        if (_state != SkillState.Ready) return false;
        if (_skillTrigger != null && !_skillTrigger.check()) return false;
        actualCooldown = cooldown;
        actualCastingTime = castingTime;
        _state = SkillState.InExecution;

        if (GetComponent<CharController>() != null)
        {
            networkAnimator.PlayAnimation(skillName);
        }
        return true;
    }

    void Update()
    {
        if (_state == SkillState.Ready || _state == SkillState.Channeling) return;

        if (_state == SkillState.InExecution)
        {
            actualCastingTime -= Time.deltaTime;
            if (actualCastingTime <= 0)
                _state = SkillState.Active;
        }

        if (_state == SkillState.Active)
        {
            foreach (Modifier modifier in modifiers)
                modifier.Execute();
            _state = (isAura || isSpot) ? SkillState.Channeling : SkillState.OnCooldown;
        }

        if (_state == SkillState.OnCooldown)
        {
            actualCooldown -= Time.deltaTime;
            if (actualCooldown <= 0)
            {
                _state = SkillState.Ready;
            }
        }
    }

    public void DeactivateAura(string auraName)
    {
        foreach (Modifier aura in modifiers)
            aura.deactivateAuraSensitive(auraName);
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

            List<TargetType> compareTypes = new List<TargetType> { TargetType.Hero, TargetType.Minion, TargetType.Spot, TargetType.Valve, TargetType.Dead };
            List<TargetType> targetTypes;
            string[] fieldStrings;
            string field, target, valueType, targetType, parsedValue;

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
                    _skillTrigger = new SkillTrigger(this, targetTypes, targetType);
                    break;

                case "instant":
                    _skillTrigger = new SkillTrigger(this);
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
                        parsedValue = skill.ChildNodes[2].InnerText;
                        valueType = (skill.ChildNodes[2] as XmlElement).GetAttribute("type");
                        target = skill.ChildNodes[3].InnerText;
                        targetType = skill.ChildNodes[4].InnerText;
                        modifiers.Add(new Modifier(this, field, parsedValue, valueType, target, targetType));
                        break;

                    case "buff":
                        field = skill.ChildNodes[1].InnerText;
                        target = skill.ChildNodes[2].InnerText;
                        targetType = skill.ChildNodes[3].InnerText;
                        modifiers.Add(new Modifier(this, field, target, targetType, false));
                        break;

                    case "debuff":
                        field = skill.ChildNodes[1].InnerText;
                        target = skill.ChildNodes[2].InnerText;
                        targetType = skill.ChildNodes[3].InnerText;
                        modifiers.Add(new Modifier(this, field, target, targetType, true));
                        break;

                    case "aura":
                        isAura = true;
                        field = skill.ChildNodes[1].InnerText;
                        fieldStrings = skill.ChildNodes[2].InnerText.Split(new string[] { ", " }, System.StringSplitOptions.None);
                        targetTypes = new List<TargetType>();
                        foreach (string type in fieldStrings)
                            foreach (TargetType compareType in compareTypes)
                                if (compareType.ToString() == type)
                                    targetTypes.Add(compareType);
                        targetType = skill.ChildNodes[3].InnerText;
                        modifiers.Add(new Modifier(this, field, targetTypes, targetType, skill.ChildNodes[4].InnerText, skill.ChildNodes[5].InnerText, (skillNode.HasAttribute("auraOnce") && skillNode.GetAttribute("auraOnce") == "true")));
                        break;

                    case "spot":
                        isSpot = true;
                        modifiers.Add(new Modifier(this, skill.ChildNodes[1].InnerText));
                        break;
                }
            }
            if (typeList.Count > 1 && isAura) isAura = false;
            if (skillNode.HasAttribute("passive") && skillNode.GetAttribute("passive") == "true")
                isPassive = true;
        }
    }
}