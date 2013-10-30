using UnityEngine;

public class MenuButton : MonoBehaviour
{

    public dfButton Left;
    public dfButton Right;
    public dfButton Up;
    public dfButton Down;

    public dfButton this[string name]
    {
        get
        {
            switch (name)
            {
                case ("Left"):
                    return Left;
                    break;
                case ("Right"):
                    return Right;
                case ("Up"):
                    return Up;
                case ("Down"):
                    return Down;
                default:
                    return Right;
            }
        }
    }
}
