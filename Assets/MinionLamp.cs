using UnityEngine;

public class MinionLamp : MonoBehaviour
{
    [SerializeField]
    public GameObject[] lamp;

    public float maxFlare = 0.5f;

    private GameObject _flareEffect;
    private Team _team;
    private bool _switchedOn = false;
    private Color _color;



    void Awake()
    {
        _team = GetComponent<Team>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_team)
            SetTeam();
       // if (!_switchedOn && transform.parent.networkView.isMine && GetComponent<Bu>() == _trophy.trophyLevel)
         //   transform.parent.networkView.RPC("TurnLightOn", RPCMode.AllBuffered, trophyLevel - 1);
    }

    private void SetTeam()
    {
        if (_team.ID == Team.TeamIdentifier.NoTeam)
            return;
        _color = Team.teamColors[(int)_team.ID];
        foreach (GameObject l in lamp)
        {
            l.renderer.material.SetColor("_Color", _color);
        }
        
        _team = null;
    }

    [RPC]
    public void TurnLightOn()
    {

        if (_switchedOn)
            return;
        Object prefab = Resources.Load("flare_lamp");
        _flareEffect = (GameObject)Instantiate(prefab);
        _flareEffect.GetComponent<RemoteTransform>().remoteTransform = GetComponent<RemoteTransform>().remoteTransform;
        _flareEffect.GetComponent<LensFlare>().color = _color;


        foreach (GameObject l in lamp)
        {
            l.renderer.material = (Material)Resources.Load("MinionLampGlow");
            l.renderer.material.SetColor("_Color", _color);
        }
        
        _switchedOn = true;
    }

    [RPC]
    public void TurnLightOff()
    {
        if (!_switchedOn)
            return;
        RemoveFlare();

        foreach (GameObject l in lamp)
        {
            l.renderer.material = (Material)Resources.Load("MinionLampOff");
            l.renderer.material.SetColor("_Color", _color);
        }
        _switchedOn = false;
    }

    private void RemoveFlare()
    {
        if(_flareEffect)
            Destroy(_flareEffect);
    }

    private void OnDestroy()
    {
        RemoveFlare();
    }

    [RPC]
    public void SetFlare(float value)
    {
        if (!_flareEffect)
            return;

        _flareEffect.GetComponent<LensFlare>().brightness = Mathf.Clamp(value,0,maxFlare);
        if (networkView.isMine)
            networkView.RPC("SetFlare", RPCMode.OthersBuffered, value);

    }
}
