using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioLibrary : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<string, AudioSource> aSources { get; private set; }

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        aSources = new Dictionary<string, AudioSource>();
        foreach (AudioSource audioSource in GetComponents<AudioSource>())
            aSources.Add(audioSource.clip.name, audioSource);
    }

    public void StartSound(string name, float delay)
    {
        if (aSources == null) Init();
        if (AbortMethod(name)) return;

        aSources[name].PlayDelayed(delay);
    }

    public void StopSound(string name)
    {
        if (aSources == null) Init();
        if (AbortMethod(name)) return;

        aSources[name].Stop();
    }

    private bool AbortMethod(string name)
    {
        if (string.IsNullOrEmpty(name) || !aSources.ContainsKey(name))
        {
            Debug.Log("RPC in AudioLibrary did not find sound: " + name + " in gameObject " + gameObject.name + ". Aborting method.");
            return true;
        }
        return false;
    }
}
