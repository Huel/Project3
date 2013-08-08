using UnityEngine;

public enum TargetType { Hero, Minion, Valve, Spot, Checkpoint, Base, Dead };

public class Target : MonoBehaviour
{

    public TargetType type;
    private TargetType _oldType;

    public Vector3 Position { set { transform.position = value; } get { return transform.position; } }

    float GetDistance(Target target)
    {
        return GetDistance(target.gameObject.transform.position);
    }

    public float GetDistance(Vector3 position)
    {
        return (position - Position).magnitude;
    }

    [RPC]
    public void SwitchTargetType(int type)
    {
        _oldType = this.type;
        this.type = (TargetType)type;
        if (networkView.isMine)
        {
            networkView.RPC("SwitchTargetType", RPCMode.OthersBuffered, type);
        }
    }

    [RPC]
    public void RestoreOldType()
    {
        this.type = _oldType;
        if (networkView.isMine)
        {
            networkView.RPC("RestoreOldType", RPCMode.OthersBuffered);
        }
    }

    public bool IsMinion()
    {
        return (type == TargetType.Minion || _oldType == TargetType.Minion);
    }
}