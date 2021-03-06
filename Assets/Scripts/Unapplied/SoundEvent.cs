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
        manager = GameObject.FindGameObjectWithTag(Tags.soundManager).GetComponent<SoundController>();
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
