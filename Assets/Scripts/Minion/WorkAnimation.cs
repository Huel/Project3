using UnityEngine;

public class WorkAnimation : MonoBehaviour
{
    [SerializeField]
    public Transform[] points;

    public GameObject[] minions;
    private bool[] move;
    private int[] lastTargets;
    private int[] targetIDs;

    public float _completeDistance;
    private float _distancePerSecond;
    // ++++++++++++++++++++++++++
    private float completeTime = 10f;
    // ++++++++++++++++++++++++++
    public float _timeDistanceToToMinion;
    private float _enqueueTimer;
    private bool _pointsSnapped = false;

    public float getCompleteTime() { return completeTime; }

    public bool Move(GameObject minion)
    {
        for (int i = 0; i < minions.Length; i++)
        {
            if (minion == minions[i])
                return move[i];
        }
        //DebugStreamer.message = "minion" + minion.networkView.viewID + "not found";
        return false;
    }

    public float EnqueueTimer() { return _enqueueTimer; }
    public float TimeDistance() { return _timeDistanceToToMinion; }

    void Awake ()
    {
        minions = new GameObject[] { null, null, null, null, null };
        move = new bool[] { false, false, false, false, false };
        lastTargets = new int[] { 0, 0, 0, 0, 0 };
        targetIDs = new int[] { 0, 0, 0, 0, 0 };
        TrackPath();
    }

    public void PrepareMinion(GameObject minion)
    {
        for (int i = 0; i < minions.Length; i++)
            if (minions[i] == null)
            {
                minions[i] = minion;
                move[i] = false;
                if (GetComponent<Team>().isEnemy(GetComponent<Valve>()._occupant))
                {
                    lastTargets[i] = 0;
                    targetIDs[i] = 1;
                }
                else
                {
                    lastTargets[i] = 1;
                    targetIDs[i] = 0;   
                }
                minions[i].GetComponent<NavMeshAgent>().enabled = false;
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
        move[id] = true;
        Vector3 newPosition = points[0].position;
        newPosition.y = minions[id].transform.position.y;
        minions[id].transform.position = newPosition;
        minions[id].transform.rotation = points[0].rotation;  
    }
	
    // Update is called once per frame
    void Update ()
    {
        Enqueue();
        for (int i = 0; i < minions.Length; i++)
        {
            if (minions[i] != null && move[i])
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
                    if (GetComponent<Team>().isEnemy(GetComponent<Valve>()._occupant) && targetIDs[i] == 1
                        || GetComponent<Team>().isOwnTeam(GetComponent<Valve>()._occupant) && targetIDs[i] == 2)
                        GetComponent<ValveMotion>().Motion();
                    if (!move[i])
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
        float distance = Time.deltaTime*_distancePerSecond;
        Vector3 direction = points[targetIDs[id]].position - minions[id].transform.position;

        while (direction.magnitude < distance)
        {
            distance -= direction.magnitude;
            lastTargets[id] = targetIDs[id];
            if (GetComponent<Team>().isEnemy(GetComponent<Valve>()._occupant))
                targetIDs[id] = (targetIDs[id] + 1) % points.Length;
            else
                targetIDs[id] = (targetIDs[id] - 1 + points.Length) % points.Length;

            direction = points[targetIDs[id]].position - points[lastTargets[id]].position;
            minions[id].transform.position = points[lastTargets[id]].position;
        }
        minions[id].transform.position += direction.normalized*distance;
        Vector3 dirA = new Vector3();
        Vector3 dirB = new Vector3();

        if (GetComponent<Team>().isEnemy(GetComponent<Valve>()._occupant))
        {
            dirA = points[lastTargets[id]].forward;
            dirB = points[targetIDs[id]].forward;
        }
        else
        {
            dirA = -points[lastTargets[id]].forward;
            dirB = -points[targetIDs[id]].forward;
        }
        
        float distanceFromLastTarget = (minions[id].transform.position - points[lastTargets[id]].position).magnitude;
        Vector3 lookDirection = Vector3.Lerp(dirA, dirB, distanceFromLastTarget/direction.magnitude);
        lookDirection.y = 0;
        minions[id].transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    public void RemoveMinion(GameObject minion)
    {
        for (int i = minions.Length-1; i >= 0; i--)
        {
            if (minion == minions[i])
            {
                move[i] = false;
                lastTargets[i] = 0;
                targetIDs[i] = 0;
                minions[i].GetComponent<NavMeshAgent>().enabled = true;
                minions[i] = null;
                break;
            }
        }
    }
}
