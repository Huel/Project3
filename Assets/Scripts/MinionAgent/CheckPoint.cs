using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Target checkPointA;
    public Target checkPointB;

    public Target GetNextCheckPoint(Target origin)
    {
        return origin == checkPointA ? checkPointB : checkPointA;
    }
}
