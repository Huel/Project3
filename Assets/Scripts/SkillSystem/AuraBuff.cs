using System.Collections.Generic;
using UnityEngine;

public class AuraBuff : MonoBehaviour
{
    private Range aura = null;

    public Skill skill;
    public List<TargetType> types;
    public List<Team.TeamIdentifier> IDs;

    private float minValue = 1;
    private float radius = 10;

    private List<Target> targets;

    void Awake()
    {
        types.Add(TargetType.Minion);
        IDs.Add(GetComponent<Team>().ID);

        aura.SetRelevantTargetTypes(types);
        aura.SetRelevantTargetTeams(IDs);
        aura.AddListener(RangeEvent.EnterRange, OnEnter);
        aura.AddListener(RangeEvent.ExitRange, OnExit);
    }

    private void OnEnter(Target target)
    {
        targets.Add(target);
    }

    private void OnExit(Target target)
    {
        targets.Remove(target);
    }

    //public void RemoveEffects()
    //{
    //    targets = aura.GetTargetsByTypesAndTeam(types, IDs);
    //    foreach (Target target in targets)
    //        foreach (BuffBehaviour buff in target.gameObject.GetComponents<BuffBehaviour>())
    //            if (buff.buffID == buffName)
    //                buff.Remove();
    //}

    void Update()
    {

    }
}
