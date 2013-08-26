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
    public float completeTime = 8f;

    public float _timeDistanceToToMinion;
    public float enqueueTimer;

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
                lastTargets[i] = 0;
                targetIDs[i] = 1;
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
        enqueueTimer += Time.deltaTime;
        if (enqueueTimer >= _timeDistanceToToMinion)
        {
            for (int i = 0; i < minions.Length; i++)
                if (minions[i] != null && !move[i])
                {
                    SnapMinion(i);
                    break;
                }
            enqueueTimer -= _timeDistanceToToMinion;
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
            targetIDs[id] = (targetIDs[id] + 1) % points.Length;
            direction = points[targetIDs[id]].position - points[lastTargets[id]].position;
            minions[id].transform.position = points[lastTargets[id]].position;
        }
        minions[id].transform.position += direction.normalized * distance;

        Quaternion rotA = points[lastTargets[id]].rotation;
        Quaternion rotB = points[targetIDs[id]].rotation;
        float distanceFromLastTarget = (minions[id].transform.position - points[lastTargets[id]].position).magnitude;
        minions[id].transform.rotation = Quaternion.Lerp(rotA, rotB, distanceFromLastTarget / direction.magnitude);
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
