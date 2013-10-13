using System.Collections.Generic;
using UnityEngine;

public class AuraBuff : MonoBehaviour
{
    public Range aura;

    public List<TargetType> types;
    public List<Team.TeamIdentifier> IDs;

    private float minValue = 1;
    private float radius = 10;

    //private List<Target> targets;

    void Awake()
    {
        types.Add(TargetType.Minion);
        IDs.Add(transform.parent.GetComponent<Team>().ID);

        aura.SetRelevantTargetTypes(types);
        aura.SetRelevantTargetTeams(IDs);
        aura.AddListener(RangeEvent.EnterRange, OnEnter);
        aura.AddListener(RangeEvent.ExitRange, OnExit);
    }

    private void OnEnter(Target target)
    {
        target.GetComponent<MinionAgent>().Buff = true;
    }

    private void OnExit(Target target)
    {
        target.GetComponent<MinionAgent>().Buff = false;
    }

    void Update()
    {

    }
}
