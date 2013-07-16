using UnityEngine;

public class HitAnimationLogic : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }



    [RPC]
    public void HitArmAnimationPlay()//int targetID)
    {
        transform.FindChild("Schlagarm_Pivot").animation.Play();

        if (networkView.isMine)
        {
            networkView.RPC("HitArmAnimationPlay", RPCMode.Others);
        }
    }

    [RPC]
    public void HitArmAnimationRewind()//int targetID)
    {
        transform.FindChild("Schlagarm_Pivot").animation.Rewind();

        if (networkView.isMine)
        {
            networkView.RPC("HitArmAnimationRewind", RPCMode.Others);
        }
    }
}
