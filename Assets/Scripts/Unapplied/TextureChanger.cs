using UnityEngine;

public class TextureChanger : MonoBehaviour
{
    public Texture defaultTexture;
    public Texture defaultTextureReduced;
    
    public Texture textureTeam1_1;
    public Texture textureTeam1_1Reduced;
    public Texture textureTeam1_2;
    public Texture textureTeam1_2Reduced;

    public Texture textureTeam2_1;
    public Texture textureTeam2_1Reduced;
    public Texture textureTeam2_2;
    public Texture textureTeam2_2Reduced;

    private Team team;
    private bool textureIsSet = false;
    public GameObject reducedMesh;

    // Use this for initialization
    void Start()
    {
        team = transform.parent.parent.GetComponent<Team>();
        renderer.material.SetTexture("_MainTex", defaultTexture);
    }

    void Update()
    {
        if (team.ID == Team.TeamIdentifier.NoTeam || textureIsSet)
            return;
        int tex = (int)(Random.value * 2);
        switch (team.ID)
        {
            case Team.TeamIdentifier.Team1:
                switch (tex)
                {
                    case 1:
                        renderer.material.SetTexture("_MainTex", textureTeam1_1);
                        if (reducedMesh != null) reducedMesh.renderer.material.SetTexture("_MainTex", textureTeam1_1Reduced);
                        break;
                    default:
                        renderer.material.SetTexture("_MainTex", textureTeam1_2);
                        if (reducedMesh != null) reducedMesh.renderer.material.SetTexture("_MainTex", textureTeam1_2Reduced);

                        break;
                }
                break;
            case Team.TeamIdentifier.Team2:
                switch (tex)
                {
                    case 1:
                        renderer.material.SetTexture("_MainTex", textureTeam2_1);
                        if (reducedMesh != null) reducedMesh.renderer.material.SetTexture("_MainTex", textureTeam2_1Reduced);
                        break;
                    default:
                        renderer.material.SetTexture("_MainTex", textureTeam2_2);
                        if (reducedMesh != null) reducedMesh.renderer.material.SetTexture("_MainTex", textureTeam2_2Reduced);
                        break;
                }
                break;
        }
        textureIsSet = true;
    }
}
