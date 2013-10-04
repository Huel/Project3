using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad : MonoBehaviour 
{
    public List<GameObject> squadMembers = new List<GameObject>();

    public void AddSquadMember(GameObject member)
    {
        squadMembers.Add(member);
        member.GetComponent<MinionAgent>().Manipulate("AddSquad", "", gameObject.GetComponent<Target>());
    }

    public bool HasSquadMember(GameObject member)
    {
        return (squadMembers.Contains(member));
    }

    public void RemoveSquadMember(GameObject member)
    {
        member.GetComponent<MinionAgent>().Manipulate("RemoveSquad", "", null);
        squadMembers.Remove(member);
    }

    public bool CanAdd()
    {
        return squadMembers.Count < 5;
    }

    public bool CanRemove()
    {
        return squadMembers.Count > 0;
    }

    void update()
    {
        for (int j = squadMembers.Count - 1; j >= 0; j--)
            if (j < squadMembers.Count)
                if (squadMembers[j] == null || squadMembers[j].GetComponent<Target>().type == TargetType.Dead)
                    squadMembers.RemoveAt(j);
    }
}
