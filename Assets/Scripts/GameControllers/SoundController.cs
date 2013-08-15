using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour
{
    public enum CorrectSound{First, Second, Third}
    private CorrectSound correctSound = CorrectSound.First;
    private bool[] triggeredLanes = new bool[3];
    private AudioSource[] sounds;
    private Bomb[] bombs = new Bomb[3];

    private bool AnyBombExploded
    {
        get
        {
            for (int i = 0; i < 3; i++)
            {
                if (!bombs[i].GameOver) continue;
                triggeredLanes[i] = false;
                return true;
            }
            return false;
        }
    }

    private bool AnyBombTriggered
    {
        get 
        { 
            bool check = false;
            foreach (bool triggeredLane in triggeredLanes)
            {
                check = triggeredLane;
            }
            return check;
        }
    }

    private bool AnySoundPlaying
    {
        get
        {
            bool check = false;
            foreach (AudioSource triggeredLane in sounds)
            {
                if (triggeredLane.isPlaying)
                {
                    check = triggeredLane.isPlaying;
                }
            }
            return check;
        }
    }

	// Use this for initialization
	void Start () 
    {
	    sounds = new AudioSource[3];
	    for (int i = 0; i < 3; i++)
        {
            sounds[i] = GameObject.Find("soundplayer_"+(i+1)).GetComponent<AudioSource>();
	    }
        sounds[0].Play();

	    for (int i = 0; i < 3; i++)
	    {
	        bombs[i] = GameObject.Find("bomb_lane0" + (i + 1)).GetComponent<Bomb>();
	    }
    }

    void Update()
    {
        if (AnySoundPlaying) return;

        if (!AnyBombExploded)
        {
            Debug.Log("No Bomb exploded");
            if (AnyBombTriggered)
            {
                Debug.Log("A Bomb triggered");
                sounds[2].Play();
            }
            else
            {
                Debug.Log("No Bomb triggered");
                sounds[0].Play();
            }
        }
        else
        {
            Debug.Log("A Bomb exploded");
            if (AnyBombTriggered)
            {
                Debug.Log("A Bomb triggered");
                sounds[2].Play();
            }
            else
            {
                Debug.Log("No Bomb Triggered");
                sounds[1].Play();
            }
        }
        Debug.Log(Random.Range(0f, 100000f));
    }



    public void TryToPlaySound(SoundEvent.Lane lane)
    {
        triggeredLanes[(int) lane] = true;
    }

    public void TryToStopSound(SoundEvent.Lane lane)
    {
        triggeredLanes[(int)lane] = false;
    }
}
