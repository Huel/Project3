using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class Bomb : MonoBehaviour
{
    public double m_InterpolationBackTime = 0.1;
    public double m_ExtrapolationLimit = 0.5;
    public GameObject WaypointA;
    public GameObject WaypointB;
    private const float forceMultiplier = 0.01f;
    private float movementSpeed;
    private GameObject towardsDestination;
    public List<GameObject> valvesA = new List<GameObject>();
    public List<GameObject> valvesB = new List<GameObject>();
    private GameObject target;

    public GameObject explosionBase1;
    public GameObject explosionBase2;

    private float ForceA
    {
        get { return (valvesA.Sum(valve => valve.GetComponent<Valve>().State) * forceMultiplier); }
    }

    private float ForceB
    {
        get { return (valvesB.Sum(valve => valve.GetComponent<Valve>().State) * forceMultiplier); }
    }

    private float Speed
    {
        get { return movementSpeed * Time.deltaTime * (ForceA < ForceB ? ForceB - ForceA : ForceA - ForceB); }
    }

    private bool BombDoesntMove
    {
        get { return ForceA - ForceB == 0f; }
    }


    internal struct State
    {
        internal double timestamp;
        internal Vector3 pos;
        internal Quaternion rot;
    }

    State[] m_BufferedState = new State[20];
    int m_TimestampCount;
    private bool gameOver = false;

    public bool GameOver
    {
        get { return gameOver; }
    }

    void Start()
    {
        movementSpeed = float.Parse(new XMLReader("Bomb.xml").GetXML().GetElementsByTagName("speed")[0].InnerText);
        towardsDestination = new GameObject();
    }

    void Update()
    {
        if (!networkView.isMine)
        {
            SmoothNetworkMovement();
            return;
        }
        if (BombDoesntMove || gameOver) return;

        target = WaypointA;
        if (ForceA > ForceB) target = WaypointB;

        if (!HaveReached(target)) { MoveTowards(target); }
        else
        {
            if (CanIPassValve(target)) SwitchStatus(target);
        }
    }

    private void SwitchStatus(GameObject waypoint)
    {
        if (!CanIPassValve(waypoint)) return;

        if (waypoint == WaypointB)
        {
            WaypointA = WaypointB;
            WaypointB = WaypointB.GetComponent<BombWaypoint>().WaypointB;
            if (WaypointB == null)
            {
                CountPoints((int)Team.TeamIdentifier.Team1);
                Network.Instantiate(Resources.Load("detonator"), explosionBase2.transform.position, explosionBase2.transform.rotation, 1);
            }
        }
        else
        {
            WaypointB = WaypointA;
            WaypointA = WaypointA.GetComponent<BombWaypoint>().WaypointA;
            if (WaypointA == null)
            {
                CountPoints((int)Team.TeamIdentifier.Team2);
                Network.Instantiate(Resources.Load("detonator"), explosionBase1.transform.position, explosionBase1.transform.rotation, 1);
            }
        }
    }

    private bool CanIPassValve(GameObject target)
    {
        if (target == WaypointB) return WaypointB.GetComponent<BombWaypoint>().GetAllowPassage() || WaypointB.GetComponent<BombWaypoint>().BombStopper.GetComponent<Team>().ID == Team.TeamIdentifier.Team1;
        return WaypointA.GetComponent<BombWaypoint>().GetAllowPassage() || WaypointA.GetComponent<BombWaypoint>().BombStopper.GetComponent<Team>().ID == Team.TeamIdentifier.Team2;
    }

    private void MoveTowards(GameObject target)
    {

        target.transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.position += Vector3.ClampMagnitude(target.transform.position - transform.position, Speed);
        towardsDestination.transform.position = transform.position;
        if (target == WaypointB)
        {
            towardsDestination.transform.LookAt(transform.position + transform.position - target.transform.position);
        }
        else
        {
            towardsDestination.transform.LookAt(target.transform);
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, towardsDestination.transform.rotation, 0.01f);
    }

    private void CountPoints(int team)
    {
        Network.Instantiate(Resources.Load("detonator"), transform.position, transform.rotation, 1);
        GameController gameController = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameController>();
        gameController.networkView.RPC("IncreaseTeamPoints", RPCMode.AllBuffered, team);
        Debug.Log("Punkte für" + ((Team.TeamIdentifier)team).ToString());
    }

    private bool HaveReached(GameObject target)
    {
        Vector3 a = transform.position;
        Vector3 b = target.transform.position;
        a.y = 0;
        b.y = 0;
        return Mathf.Abs((b - a).magnitude) < 0.5f;
    }

    public virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
        }
        else
        {
            Vector3 pos = Vector3.zero;

            Quaternion rot = Quaternion.identity;

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);

            for (int i = m_BufferedState.Length - 1; i >= 1; i--)
            {
                m_BufferedState[i] = m_BufferedState[i - 1];
            }

            State state = new State();
            state.timestamp = info.timestamp;
            state.pos = pos;
            state.rot = rot;

            m_BufferedState[0] = state;

            m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

            for (int i = 0; i < m_TimestampCount - 1; i++)
            {
                if (m_BufferedState[i].timestamp < m_BufferedState[i + 1].timestamp)
                    Debug.Log("State inconsistent");
            }
        }
    }

    private void SmoothNetworkMovement()
    {
        if (networkView.isMine)
            return;

        double interpolationTime = Network.time - m_InterpolationBackTime;

        if (m_BufferedState[0].timestamp > interpolationTime)
        {
            for (int i = 0; i < m_TimestampCount; i++)
            {
                if (!(m_BufferedState[i].timestamp <= interpolationTime) && i != m_TimestampCount - 1) continue;

                State rhs = m_BufferedState[Mathf.Max(i - 1, 0)];

                State lhs = m_BufferedState[i];

                double length = rhs.timestamp - lhs.timestamp;
                float t = 0.0F;

                if (length > 0.0001)
                    t = (float)((interpolationTime - lhs.timestamp) / length);

                transform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
                transform.localRotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);
                return;
            }
        }
        else
        {
            State latest = m_BufferedState[0];

            float extrapolationLength = (float)(interpolationTime - latest.timestamp);

            if (extrapolationLength < m_ExtrapolationLimit)
            {
                Quaternion angularRotation = Quaternion.identity;

                transform.localPosition = latest.pos + new Vector3(0f, 0f, extrapolationLength);
                transform.localRotation = angularRotation * latest.rot;
            }
        }
    }

    public int BombDirection()
    {
        if (ForceA > ForceB)
        {
            return (int)Team.TeamIdentifier.Team1;
        }

        if (ForceA < ForceB)
        {
            return (int)Team.TeamIdentifier.Team2;
        }

        return -1;
    }
}
