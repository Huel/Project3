using UnityEngine;

public class SoundEvent : MonoBehaviour
{
    public enum SoundFunction { PlayMusic, StopMusic }
    public enum Lane { Lane1, Lane2, Lane3 }
    public SoundFunction myJob = SoundFunction.PlayMusic;
    public Lane myLane = Lane.Lane1;
    private SoundController manager;
    private int count = 0;

    // Use this for initialization
    void Start()
    {
        manager = GameObject.Find("sound_manager").GetComponent<SoundController>();
    }

    void OnTriggerEnter(Collider other)
    {

        if (myJob == SoundFunction.PlayMusic)
        {
            Debug.Log("A trigger just entered a playmusic field on lane " + ((int)myLane));
            manager.TryToPlaySound(myLane);
        }
        else
        {
            Debug.Log("A trigger just entered a stopmusic field on lane " + ((int)myLane));
            manager.TryToStopSound(myLane);
        }
    }
}
