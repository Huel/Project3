using UnityEngine;

public class SoundController : MonoBehaviour
{
    private bool[] triggeredLanes = new bool[3];
    private AudioSource[] sounds = new AudioSource[3];
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
            return (triggeredLanes[0] || triggeredLanes[1] || triggeredLanes[2]);
        }
    }

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            sounds[i] = GameObject.Find("soundplayer_" + (i + 1)).GetComponent<AudioSource>();
        }

        for (int i = 0; i < 3; i++)
        {
            bombs[i] = GameObject.Find("bomb_lane0" + (i + 1)).GetComponent<Bomb>();
        }
    }

    void Update()
    {
        Debug.Log(triggeredLanes[0] + " " + triggeredLanes[1] + " " + triggeredLanes[2]);
        if (!AnyBombExploded)
        {
            sounds[1].mute = !AnyBombTriggered;
        }
        else
        {
            sounds[1].mute = false;
        }
        sounds[2].mute = !AnyBombTriggered;
    }

    /// <summary>
    /// Used by SoundEvents, tells the controller which lane has triggered enter-proximity sounds for the bomb.
    /// </summary>
    /// <param name="lane">The lane of the bomb, which triggered the enter-proximity warning.</param>
    public void TryToPlaySound(SoundEvent.Lane lane)
    {
        Debug.Log("tryto play sound");
        triggeredLanes[(int)lane] = true;
    }

    /// <summary>
    /// Used by SoundEvents, tells the controller which lane has triggered leave-proximity sounds for the bomb.
    /// </summary>
    /// <param name="lane">The lane of the bomb, which triggered the leave-proximity warning.</param>
    public void TryToStopSound(SoundEvent.Lane lane)
    {
        Debug.Log("tryto stop sound");
        triggeredLanes[(int)lane] = false;
    }
}
