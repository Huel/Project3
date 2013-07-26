using UnityEngine;

public enum TargetType { Hero, Minion, Valve, Spot, Checkpoint, Base, Dead };

public class Target : MonoBehaviour
{

    public TargetType type;

    public Vector3 Position { set { transform.position = value; } get { return transform.position; } }

    float GetDistance(Target target)
    {
        return GetDistance(target.gameObject.transform.position);
    }

    public float GetDistance(Vector3 position)
    {
        return (position - Position).magnitude;
    }
}