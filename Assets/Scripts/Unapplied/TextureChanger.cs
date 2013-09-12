using UnityEngine;

public class TextureChanger : MonoBehaviour
{

    private Team _team;

    void Awake()
    {
        _team = transform.parent.GetComponent<Team>();
    }

    void Update()
    {
        if (_team.ID == Team.TeamIdentifier.NoTeam)
            return;
        renderer.material.SetColor("_Team", Team.teamColors[(int)_team.ID]);
        Destroy(this);
    }
}
