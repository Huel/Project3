using UnityEngine;
using System.Collections;

public class LampController : MonoBehaviour {


    private GameObject[] _flareEffects = new GameObject[3];

    public TrophyLamp lamp1;
    public TrophyLamp lamp2;
    public TrophyLamp lamp3;
    private TrophyLamp[] lamps;

    void Awake()
    {
        lamps = new TrophyLamp[] {lamp1, lamp2, lamp3};
    }

    [RPC]
    public void TurnLightOn(int lamp)
    {
        if(_flareEffects[lamp])
            return;
        Object prefab = Resources.Load("flare_lamp");
        _flareEffects[lamp] = (GameObject)Instantiate(prefab);
        _flareEffects[lamp].GetComponent<RemoteTransform>().remoteTransform = lamps[lamp].GetComponent<RemoteTransform>().remoteTransform;
        _flareEffects[lamp].GetComponent<LensFlare>().color = Team.teamColors[(int) GetComponent<Team>().ID];
        lamps[lamp].TurnLightOn();
    }

    void OnDestroy()
    {
        foreach (GameObject flareEffect in _flareEffects)
        {
            if(flareEffect)
                Destroy(flareEffect);
        }
            
    }
}
