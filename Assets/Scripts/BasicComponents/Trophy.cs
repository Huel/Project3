using UnityEngine;

public class Trophy : MonoBehaviour
{
    public int trophyLevel = 0;

    [RPC]
    public void IncTrophyLevel()
    {
        if (networkView.isMine)
            trophyLevel++;
    }
}