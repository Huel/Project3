using UnityEngine;

public class Base : MonoBehaviour
{
    public Target lane01;
    public Target lane02;
    public Target lane03;

    public Target GetCheckPoint(MinionAgent.LaneIdentifier lane)
    {
        switch (lane)
        {
            case MinionAgent.LaneIdentifier.Lane1:
                return lane01;
            case MinionAgent.LaneIdentifier.Lane2:
                return lane02;
            default:
                return lane03;
        }
    }
}
