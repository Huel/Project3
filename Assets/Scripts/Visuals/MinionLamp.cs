using UnityEngine;

public class MinionLamp : MonoBehaviour
{
    [SerializeField]
    public GameObject[] lamp;

    public float maxFlare = 0.5f;

    private GameObject _flareEffect;
    private Team _team;
    private bool _switchedOn = false;
    private bool _mission;
    private Color _color;

    private float frequencytimer;
    private bool on = false;

    public bool getSwitchOn() { return _switchedOn; }

    private AudioLibrary library;
    private string sound;

    void Awake()
    {
        _team = GetComponent<Team>();
        library = transform.FindChild("sound_minion").GetComponent<AudioLibrary>();
        sound = new XMLReader("Minion.xml").GetXML().GetElementsByTagName("auraBuff")[0].InnerText;
    }

    // Update is called once per frame
    void Update()
    {
        if (_team)
            SetTeam();

        if (_mission)
        {
            Kamikaze(); 
            return;
        }

        if (!_switchedOn && networkView.isMine && GetComponent<MinionAgent>().Buff)
        {
            networkView.RPC("TurnLightOn", RPCMode.AllBuffered);
        }
        else if (_switchedOn && networkView.isMine && !GetComponent<MinionAgent>().Buff)
        {
            networkView.RPC("TurnLightOff", RPCMode.AllBuffered);
        }
    }

    private void Kamikaze()
    {
        frequencytimer += Time.deltaTime;
        if (frequencytimer > 0.33f)
        {
            on = !on;
            if (on)
            {
                SetFlare(0);
                foreach (GameObject l in lamp)
                {
                    l.renderer.material = (Material)Resources.Load("MinionLampOff");
                    l.renderer.material.SetColor("_Color", _color);
                }
            }
            else
            {
                SetFlare(1);
                foreach (GameObject l in lamp)
                {
                    l.renderer.material = (Material)Resources.Load("MinionLampGlow");
                    l.renderer.material.SetColor("_Color", _color);
                }
            }
            frequencytimer = 0;
        }
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

        library.StartSound(sound);
    }

    [RPC]
    public void TurnLightOff()
    {
        if (!_switchedOn)
            return;

        if (_mission) return;

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
        if (_flareEffect)
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

        _flareEffect.GetComponent<LensFlare>().brightness = Mathf.Clamp(value, 0, maxFlare);
        if (networkView.isMine)
            networkView.RPC("SetFlare", RPCMode.OthersBuffered, value);
    }

    [RPC]
    public void KamikazeStart()
    {
        if (!_flareEffect)
            return;
        _mission = true;
        _flareEffect.GetComponent<LensFlare>().brightness = Mathf.Clamp(2, 0, maxFlare*2);
        
        if (networkView.isMine)
            networkView.RPC("KamikazeStart", RPCMode.OthersBuffered);
    }
}
