using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioLibrary : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<string, AudioSource> aSources { get; private set; }

    void Start()
    {
        aSources = new Dictionary<string, AudioSource>();
        foreach (AudioSource audioSource in GetComponents<AudioSource>())
            aSources.Add(audioSource.clip.name, audioSource);
    }

    /// <summary>
    /// Tries to play sound. Return false if it fails, true if it succeeds.
    /// </summary>
    /// <param name="name">Name of the Sound file, should be extracted from a XML!</param>
    public void PlaySound(string name, float delay = 0f)
    {
        networkView.RPC("StartSound", RPCMode.All, name, delay);
    }

    /// <summary>
    /// This is an RPC Method. Don't use it. Use PlaySound instead!
    /// </summary>
    [RPC]
    public void StartSound(string name, float delay)
    {
        if (AbortMethod(name)) return;

        aSources[name].PlayDelayed(delay);
    }

    private bool AbortMethod(string name)
    {
        if (string.IsNullOrEmpty(name) || !aSources.ContainsKey(name))
        {
            Debug.Log("RPC in AudioLibrary did not find sound: " + name + ". Aborting method.");
            return true;
        }
        return false;
    }
}