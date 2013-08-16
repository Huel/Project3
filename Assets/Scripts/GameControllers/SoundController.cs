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
            return (triggeredLanes[0]||triggeredLanes[1]||triggeredLanes[2]);
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

	    for (int i = 0; i < 3; i++)
	    {
	        bombs[i] = GameObject.Find("bomb_lane0" + (i + 1)).GetComponent<Bomb>();
	    }
    }

    void Update()
    {

        if (!AnyBombExploded)
        {
            sounds[1].mute = !AnyBombTriggered;
            sounds[2].mute = !AnyBombTriggered;
        }
        else
        {
            sounds[1].mute = false;
            sounds[2].mute = !AnyBombTriggered;
        }
    }



    public void TryToPlaySound(SoundEvent.Lane lane)
    {
        Debug.Log("playsound in soundcontroller used");
        triggeredLanes[(int) lane] = true;
    }

    public void TryToStopSound(SoundEvent.Lane lane)
    {
        Debug.Log("stopsound in soundcontroller used");
        triggeredLanes[(int)lane] = false;
    }
}
