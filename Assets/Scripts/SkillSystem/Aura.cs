using System.Collections.Generic;
using UnityEngine;

public class Aura : MonoBehaviour
{
    public Range aura = null;

    public bool buffIsDebuff;

    public Skill skill;
    public string buffName;
    public List<TargetType> types;
    public List<Team.TeamIdentifier> IDs;
    public float minValue = 1;
    public float radius;

    private List<Target> targets;

    public void Init(Skill skill, string buffName, List<TargetType> types, List<Team.TeamIdentifier> IDs, float minValue, float radius)
    {
        this.skill = skill;
        this.buffName = buffName;
        this.types = types;
        this.IDs = IDs;
        this.minValue = minValue;
        this.radius = radius;

        buffIsDebuff = !IDs.Contains(skill.gameObject.GetComponent<Team>().ID);

        aura.SetRelevantTargetTypes(types);
        aura.SetRelevantTargetTeams(IDs);
        aura.AddListener(RangeEvent.EnterRange, onEnter);
        aura.AddListener(RangeEvent.ExitRange, onExit);
    }

    private void onEnter(Target target)
    {
        Debug.Log("Enter:" + target);
        BuffBehaviour buff = target.gameObject.AddComponent<BuffBehaviour>();
        buff.Load(skill, buffName, buffIsDebuff, true);
    }

    private void onExit(Target target)
    {
        Debug.Log("Exit:" + target);
        foreach (BuffBehaviour buff in target.gameObject.GetComponents<BuffBehaviour>())
            if (buff.buffID == buffName)
                buff.Remove();
    }

    void Update()
    {
        if (minValue == 1) return;
        targets = aura.GetTargetsByTypesAndTeam(types, IDs);
        float value;
        foreach (Target target in targets)
        {
            value = minValue + ((1-(target.GetDistance(skill.gameObject.transform.position)/radius))* (1-minValue));
            foreach (BuffBehaviour buff in target.gameObject.GetComponents<BuffBehaviour>())
                if (buff.buffID == buffName)
                    buff.ChangeAuraValue(value);
        }
    }
}
