using UnityEngine;

public class TextureChanger : MonoBehaviour
{
    public Texture defaultTexture;

    public Texture textureTeam1_1;

    public Texture textureTeam1_2;

    // Use this for initialization
    void Start()
    {
        //int tex = (int)(Random.value * 2);

        Team team = transform.parent.parent.GetComponent<Team>();

        if(team.ID == Team.TeamIdentifier.Team1)
            renderer.material.SetTexture("_MainTex", textureTeam1_1);
        else if (team.ID == Team.TeamIdentifier.Team2)
            renderer.material.SetTexture("_MainTex", textureTeam1_2);
        else
            renderer.material.SetTexture("_MainTex", defaultTexture);
    }
}
