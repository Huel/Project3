using UnityEngine;
using System.Collections;

public enum TargetType { Hero, Minion, Valve, Spot };

public class Target : MonoBehaviour 
{
    private Vector3 position;
    private TargetType type;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    Vector3 Position { set{ position = value;} get { return position;}}

    float GetDistance(Target target)
    {
        Vector3 position = target.transform.position;
        return (Mathf.Sqrt(Mathf.Pow(Mathf.Abs(position.x - position.x), 2) + Mathf.Pow(Mathf.Abs(position.y - position.y), 2) + Mathf.Pow(Mathf.Abs(position.z - position.z), 2)));
    }

    float GetDistance(Vector3 position)
    {
        return (Mathf.Sqrt(Mathf.Pow(Mathf.Abs(this.position.x - position.x), 2) + Mathf.Pow(Mathf.Abs(this.position.y - position.y), 2) + Mathf.Pow(Mathf.Abs(this.position.z - position.z), 2)));
    }
}
