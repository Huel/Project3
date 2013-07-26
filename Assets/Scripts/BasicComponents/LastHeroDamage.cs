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
            networkView.RPC("SetSourceRPC", networkView.owner, ID);
    }

    void Update()
    {
        if (lastDamageSource == null) return;

        if (duration > 0) duration -= Time.deltaTime;

        if (duration <= 0 || !lastDamageSource.GetComponent<Health>().isAlive() || !gameObject.GetComponent<Health>().isAlive())
        {
            if (!gameObject.GetComponent<Health>().isAlive())
                lastDamageSource.GetComponent<Trophy>().IncTrophyLevel();
            duration = 15;
            lastDamageSource = null;
        }
    }
}