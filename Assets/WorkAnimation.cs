using UnityEngine;

public class WorkAnimation : MonoBehaviour
{
    [SerializeField]
    public Transform[] points;

    public GameObject minion;
    public int lastTarget;
    public int targetID;

    public float _completeDistance;
    private float _distancePerSecond;
    public float completeTime = 3f;

	void Awake ()
	{
        TrackPath();
	    SnapMinion();
	}

    private void SnapMinion()
    {
        Vector3 newPosition = points[0].position;
        newPosition.y = minion.transform.position.y;
        minion.transform.position = newPosition;
        minion.transform.rotation = points[0].rotation;

        lastTarget = 0;
        targetID = 1;

    }

    void TrackPath()
    {
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 a = points[i].position;
            Vector3 b = points[(i + 1)%points.Length].position;
            a.y = b.y = 0;
            _completeDistance += (a - b).magnitude;
        }
        _distancePerSecond = _completeDistance/completeTime;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    MoveMinion();
	}

    private void MoveMinion()
    {
        float distance = Time.deltaTime*_distancePerSecond;
        Vector3 direction = points[targetID].position - minion.transform.position;
           
        while (direction.magnitude < distance)
        {
            distance -= direction.magnitude;
            lastTarget = targetID;
            targetID = (targetID + 1)%points.Length;
            direction = points[targetID].position - points[lastTarget].position;
            minion.transform.position = points[lastTarget].position;
        }
        minion.transform.position += direction.normalized * distance;

        Quaternion rotA = points[lastTarget].rotation;
        Quaternion rotB = points[targetID].rotation;
        float distanceFromLastTarget = (minion.transform.position - points[lastTarget].position).magnitude;
        minion.transform.rotation = Quaternion.Lerp(rotA, rotB, distanceFromLastTarget / direction.magnitude);
    }
}
