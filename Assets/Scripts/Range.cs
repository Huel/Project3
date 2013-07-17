using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum RangeEvent { EnterRange, ExitRange };

public class Range : MonoBehaviour 
{
    public delegate void OnRangeEvent(Target target);

    private List<Target> objectsInRange = new List<Target>();
    private List<TargetType> relevantTargetTypes = new List<TargetType>();
    private List<OnRangeEvent> enterRangeListener = new List<OnRangeEvent>();
    private List<OnRangeEvent> exitRangeListener = new List<OnRangeEvent>();

    float Radius { set { gameObject.GetComponent<SphereCollider>().radius = value; } get { return gameObject.GetComponent<SphereCollider>().radius; } }

    public int Count()
    {
        return objectsInRange.Count;
    }

    public void AddListener(RangeEvent type, OnRangeEvent listenerFunction)
    {
        switch (type)
        {
            case RangeEvent.EnterRange:
                enterRangeListener.Add(listenerFunction);
                break;
            case RangeEvent.ExitRange:
                exitRangeListener.Add(listenerFunction);
                break;
        }
    }

    public void RemoveListener(RangeEvent type, OnRangeEvent listenerFunction)
    {
        switch (type)
        {
            case RangeEvent.EnterRange:
                enterRangeListener.Remove(listenerFunction);
                break;
            case RangeEvent.ExitRange:
                exitRangeListener.Remove(listenerFunction);
                break;
        }
    }

    public void SetRelevantTargetTypes(List<TargetType> types)
    {
        relevantTargetTypes = types;
    }

    public int CountByType(TargetType type)
    {
        int count = 0;
        foreach (Target target in objectsInRange)
            if (target.type == type) count++;
        return count;
    }

    public Target GetNearestTarget()
    {
        order();
        return objectsInRange[0];
    }

    public Target GetNearestTargetByType(TargetType type)
    {
        order();
        for (int i = 0; i < objectsInRange.Count; i++)
            if (objectsInRange[i].type == type)
                return objectsInRange[i];
        return null;
    }

    public List<Target> GetNearestTargets(int amount)
    {
        order();
        if (amount > objectsInRange.Count) amount = objectsInRange.Count;

        List<Target> targets = new List<Target>();
        for (int i = 0; i < amount; i++)
            targets.Add(objectsInRange[i]);
        return targets;
    }

    public List<Target> GetNearestTargetsByType(int amount, TargetType type)
    {
        order();
        if (amount > objectsInRange.Count) amount = objectsInRange.Count;

        List<Target> targets = new List<Target>();
        for (int i = 0; i < amount; i++)
            if (objectsInRange[i].type == type) targets.Add(objectsInRange[i]);
        return targets;
    }

    public List<Target> GetTargets()
    {
        order();
        return objectsInRange;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Target>() == null) return;
        Target target = other.gameObject.GetComponent<Target>();
        objectsInRange.Add(other.gameObject.GetComponent<Target>());
        if(relevantTargetTypes.Contains(target.type))
            foreach (OnRangeEvent listener in enterRangeListener)
                if (listener != null) listener(target);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Target>() == null) return;
        Target target = other.gameObject.GetComponent<Target>();
        if (relevantTargetTypes.Contains(target.type))
            foreach (OnRangeEvent listener in exitRangeListener)
                if (listener != null) listener(other.gameObject.GetComponent<Target>());
        objectsInRange.Remove(target);
    }

    void order()
    {
        float distance;
        int position;
        int amount = objectsInRange.Count;
        int i;
        List<Target> sortedList = new List<Target>();
        while(sortedList.Count < amount)
        {
            position = -1;
            distance = float.MaxValue;
            for(i = 0; i<objectsInRange.Count; i++)
                if ((objectsInRange[i].gameObject.transform.position - gameObject.transform.position).magnitude <= distance)
                {
                    position = i;
                    distance = (objectsInRange[i].gameObject.transform.position - gameObject.transform.position).magnitude;
                }
            sortedList.Add(objectsInRange[position]);
            objectsInRange.RemoveAt(position);
        }
        objectsInRange = sortedList;
    }
}
