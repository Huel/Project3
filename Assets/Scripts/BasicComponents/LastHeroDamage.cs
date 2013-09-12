using UnityEngine;

public class LastHeroDamage : MonoBehaviour
{
    public GameObject lastDamageSource = null;

    public float duration = 0;

    [RPC]
    public void SetSource(NetworkViewID ID)
    {
        if (networkView.isMine)
        {
            lastDamageSource = NetworkView.Find(ID).gameObject;
            duration = 15f;
        }
        else
            networkView.RPC("SetSource", networkView.owner, ID);
    }

    void Update()
    {
        if (lastDamageSource == null) return;

        if (duration > 0) duration -= Time.deltaTime;

        if (duration <= 0 || !lastDamageSource.GetComponent<Health>().IsAlive() || !gameObject.GetComponent<Health>().IsAlive())
        {
            if (!gameObject.GetComponent<Health>().IsAlive())
                lastDamageSource.networkView.RPC("IncTrophyLevel", lastDamageSource.networkView.owner);
            duration = 15;
            lastDamageSource = null;
        }
    }
}