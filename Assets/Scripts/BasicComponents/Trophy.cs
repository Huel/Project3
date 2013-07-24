using UnityEngine;
using System.Collections;

public class Trophy : MonoBehaviour
{
    public int trophyLevel = 0;

    public void IncTrophyLevel()
    {
        trophyLevel++;
    }
}