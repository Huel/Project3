using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContactTrigger : MonoBehaviour 
{
    private delegate void OnContact(Target contact);

    private List<Target> contacts;

    private List<OnContact> defaultListener = new List<OnContact>();
    private Dictionary<TargetType, List<OnContact>> typeListener = new Dictionary<TargetType, List<OnContact>>();
    private Dictionary<Target, List<OnContact>> targetListener = new Dictionary<Target, List<OnContact>>();

    public bool Contact(Target target)
    {
        return contacts.Contains(target);
    }

    public void AddListener(OnContact listenerFunction)
    {
        defaultListener.Add(listenerFunction);
    }

    public void AddListener(TargetType targetType, OnContact listenerFunction)
    {
        if(typeListener.ContainsKey(targetType))
            typeListener[targetType].Add(listenerFunction);
        else
            typeListener.Add(targetType, new List<OnContact>{listenerFunction});
    }

    public void AddListener(Target target, OnContact listenerFunction)
    {
        if (targetListener.ContainsKey(target))
            targetListener[target].Add(listenerFunction);
        else
            targetListener.Add(target, new List<OnContact> { listenerFunction });
    }

    public void RemoveListener(OnContact listenerFunction)
    {
        defaultListener.Remove(listenerFunction);
    }

    public void RemoveListener(TargetType targetType, OnContact listenerFunction)
    {
        typeListener[targetType].Remove(listenerFunction);
        if (typeListener[targetType].Count == 0)
            typeListener.Remove(targetType);
    }

    public void RemoveListener(Target target, OnContact listenerFunction)
    {
        targetListener[target].Remove(listenerFunction);
        if (targetListener[target].Count == 0)
            targetListener.Remove(target);
    }

    void OnTriggerEnter(Collider other)
    {
        contacts.Add(other.gameObject.GetComponent<Target>());
    }

    void OnTriggerExit(Collider other)
    {
        contacts.Remove(other.gameObject.GetComponent<Target>());
    }

    void OnTriggerStay(Collider other)
    {
        Target target = other.gameObject.GetComponent<Target>();

        foreach (OnContact listener in defaultListener)
            listener(target);

        if(typeListener.ContainsKey(target.type))
            foreach (OnContact listener in typeListener[target.type])
                listener(target);

        if (targetListener.ContainsKey(target))
            foreach (OnContact listener in targetListener[target])
                listener(target);
    }
}
