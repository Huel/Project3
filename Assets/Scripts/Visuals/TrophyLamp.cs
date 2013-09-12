using UnityEngine;

public class TrophyLamp : MonoBehaviour
{

    private Trophy _trophy;
    private Team _team;
    private bool _switchedOn = false;
    private Color _color;
    public int trophyLevel;



    void Awake()
    {
        _trophy = transform.parent.GetComponent<Trophy>();
        _team = transform.parent.GetComponent<Team>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_team)
            SetTeam();
        if (!_switchedOn && transform.parent.networkView.isMine && trophyLevel == _trophy.trophyLevel)
            transform.parent.networkView.RPC("TurnLightOn", RPCMode.AllBuffered, trophyLevel - 1);
    }

    private void SetTeam()
    {
        if (_team.ID == Team.TeamIdentifier.NoTeam)
            return;
        _color = Team.teamColors[(int)_team.ID];
        renderer.material.SetColor("_Color", _color);
        _team = null;
    }

    public void TurnLightOn()
    {
        renderer.material = (Material)Resources.Load("HeroLampGlow");
        renderer.material.SetColor("_Color", _color);
        _switchedOn = true;
    }


}
