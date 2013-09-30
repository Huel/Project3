using System.Collections.Generic;
using UnityEngine;

public class WorkAnimation : MonoBehaviour
{
    [SerializeField]
    public Transform[] points;

    public GameObject[] minions;
    private bool[] _move;
    private int[] _lastTargets;
    private int[] _targetIDs;

    private float _completeDistance;
    private float _distancePerSecond;
    // ++++++++++++++++++++++++++
    private float completeTime = 10f;
    // ++++++++++++++++++++++++++
    public float _timeDistanceToToMinion;
    private float _enqueueTimer;
    private bool _pointsSnapped = false;

    public float GetCompleteTime() { return completeTime; }

    public bool Move(GameObject minion)
    {
        for (int i = 0; i < minions.Length; i++)
        {
            if (minion == minions[i])
                return _move[i];
        }
        //DebugStreamer.message = "minion" + minion.networkView.viewID + "not found";
        return false;
    }

    public float EnqueueTimer() { return _enqueueTimer; }
    public float TimeDistance() { return _timeDistanceToToMinion; }

    void Awake()
    {
        minions = new GameObject[] { null, null, null, null, null };
        _move = new bool[] { false, false, false, false, false };
        _lastTargets = new int[] { 0, 0, 0, 0, 0 };
        _targetIDs = new int[] { 0, 0, 0, 0, 0 };
        TrackPath();
    }

    public void PrepareMinion(GameObject minion)
    {
        for (int i = 0; i < minions.Length; i++)
            if (minions[i] == null)
            {
                minions[i] = minion;
                _move[i] = false;
                if (GetComponent<Team>().IsEnemy(GetComponent<Valve>().Occupant))
                {
                    _lastTargets[i] = 0;
                    _targetIDs[i] = 1;
                }
                else
                {
                    _lastTargets[i] = 1;
                    _targetIDs[i] = 0;
                }
                if (minions[i].networkView.isMine)
                    minions[i].GetComponent<MinionAgent>().SetNavMeshAgent(false);
                else
                    minions[i].networkView.RPC("SetNavMeshAgent", minions[i].networkView.owner, false);
                break;
            }
    }

    void TrackPath()
    {
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 a = points[i].position;
            Vector3 b = points[(i + 1) % points.Length].position;
            a.y = b.y = 0;
            _completeDistance += (a - b).magnitude;
        }
        _distancePerSecond = _completeDistance / completeTime;

        _timeDistanceToToMinion = completeTime / minions.Length;
    }

    private void SnapMinion(int id)
    {
        if (!_pointsSnapped)
        {
            float minionY = minions[id].transform.position.y;
            foreach (Transform point in points)
            {
                Vector3 newY = point.position;
                newY.y = minionY;
                point.position = newY;
            }
        }
        _move[id] = true;
        if (GetComponent<Team>().IsEnemy(GetComponent<Valve>().Occupant))
        {
            Vector3 newPosition = points[0].position;
            newPosition.y = minions[id].transform.position.y;
            minions[id].transform.position = newPosition;
            minions[id].transform.rotation = points[0].rotation;
        }
        else
        {
            Vector3 newPosition = points[1].position;
            newPosition.y = minions[id].transform.position.y;
            minions[id].transform.position = newPosition;
            minions[id].transform.rotation = points[4].rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Enqueue();
        for (int i = 0; i < minions.Length; i++)
        {
            if (minions[i] != null && minions[i].GetComponent<MinionAgent>().GetTarget() != GetComponent<Target>())
            {
                RemoveMinion(minions[i]);
                continue;
            }
            if (minions[i] != null && _move[i])
                MoveMinion(i);
        }
    }

    private void Enqueue()
    {
        _enqueueTimer += Time.deltaTime;
        if (_enqueueTimer >= _timeDistanceToToMinion)
        {
            for (int i = 0; i < minions.Length; i++)
                if (minions[i] != null)
                {
                    if (GetComponent<Team>().IsEnemy(GetComponent<Valve>().Occupant) && _targetIDs[i] == 1
                        || GetComponent<Team>().IsOwnTeam(GetComponent<Valve>().Occupant) && _targetIDs[i] == 2)
                        GetComponent<ValveMotion>().Motion();
                    if (!_move[i])
                    {
                        SnapMinion(i);
                        break;
                    }
                }
            _enqueueTimer -= _timeDistanceToToMinion;
        }
    }

    private void MoveMinion(int id)
    {
        float distance = Time.deltaTime * _distancePerSecond;
        Vector3 direction = points[_targetIDs[id]].position - minions[id].transform.position;

        while (direction.magnitude < distance)
        {
            distance -= direction.magnitude;
            _lastTargets[id] = _targetIDs[id];
            // normal moving direction
            if (GetComponent<Team>().IsEnemy(GetComponent<Valve>().Occupant))
                _targetIDs[id] = (_targetIDs[id] + 1) % points.Length;
            // invert moving direction
            else
                _targetIDs[id] = (_targetIDs[id] - 1 + points.Length) % points.Length;

            direction = points[_targetIDs[id]].position - points[_lastTargets[id]].position;
            minions[id].transform.position = points[_lastTargets[id]].position;
        }
        minions[id].transform.position += direction.normalized * distance;
        Vector3 dirA = new Vector3();
        Vector3 dirB = new Vector3();

        // get the direction of targets
        if (GetComponent<Team>().IsEnemy(GetComponent<Valve>().Occupant))
        {
            dirA = points[_lastTargets[id]].forward;
            dirB = points[_targetIDs[id]].forward;
        }
        // get the opposite diretion
        else
        {
            dirA = -points[_lastTargets[id]].forward;
            dirB = -points[_targetIDs[id]].forward;
        }
        // transform rotation from Minion
        float distanceFromLastTarget = (minions[id].transform.position - points[_lastTargets[id]].position).magnitude;
        Vector3 lookDirection = Vector3.Lerp(dirA, dirB, distanceFromLastTarget / direction.magnitude);
        lookDirection.y = 0;
        minions[id].transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    public void RemoveMinion(GameObject minion)
    {
        for (int i = minions.Length - 1; i >= 0; i--)
        {
            if (minion == minions[i])
            {
                _move[i] = false;
                _lastTargets[i] = 0;
                _targetIDs[i] = 0;
                if (minions[i].networkView.isMine)
                    minions[i].GetComponent<MinionAgent>().SetNavMeshAgent(true);
                else
                    minions[i].networkView.RPC("SetNavMeshAgent", minions[i].networkView.owner, true);
                minions[i] = null;
                break;
            }
        }
    }

    public void CheckMinions(List<MinionAgent> minionList)
    {
        foreach (GameObject minion in minions)
        {
            if (minion != null && minionList.IndexOf(minion.GetComponent<MinionAgent>()) == -1)
            {
                RemoveMinion(minion);
            }
        }
    }
}
