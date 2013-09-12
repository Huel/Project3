using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Target checkPointA;
    public Target checkPointB;

    private Target _target;

    public void Awake()
    {
        _target = GetComponent<Target>();
    }

    public Target GetNextCheckPoint(Target origin)
    {
        return origin == checkPointA ? checkPointB : checkPointA;
    }

}
