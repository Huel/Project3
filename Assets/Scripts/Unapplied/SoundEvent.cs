using UnityEngine;
using System.Collections;

public class SoundEvent : MonoBehaviour
{
    public enum SoundFunction { PlayMusic, StopMusic }
    public enum Lane{Lane1, Lane2, Lane3}
    public SoundFunction myJob = SoundFunction.PlayMusic;
    public Lane myLane = Lane.Lane1;
    private SoundController manager;

	// Use this for initialization
	void Start ()
	{
	    manager = GameObject.Find("sound_manager").GetComponent<SoundController>();
	}
	
    void OnTriggerEnter(Collider other)
    {
        if (myJob == SoundFunction.PlayMusic)
        {
            manager.TryToPlaySound(myLane);
        }
        else
        {
            manager.TryToStopSound(myLane);
        }
    }
}
