using UnityEngine;

public class BombWaypoint : MonoBehaviour
{
    public GameObject WaypointA;
    public GameObject WaypointB;
    public GameObject BombStopper;

    public bool GetAllowPassage()
    {
        if (BombStopper != null
            && BombStopper.GetComponent<Valve>() != null
            && BombStopper.GetComponent<Valve>().GetValveState() != Valve.ValveState.Closed)
            return false;
        return true;
    }
}
