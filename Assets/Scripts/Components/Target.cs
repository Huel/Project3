using UnityEngine;
using System.Collections;

public enum TargetType { Hero, Minion, Valve, Spot };

public class Target : MonoBehaviour 
{
    private Vector3 _position;

    public TargetType type;

    Vector3 Position { set{ _position = value;} get { return _position;}}

    float GetDistance(Target target)
    {
        return GetDistance(target.gameObject.transform.position);
    }

    float GetDistance(Vector3 position)
    {
        return (position - _position).magnitude;
    }
}