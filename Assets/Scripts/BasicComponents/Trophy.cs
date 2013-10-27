using UnityEngine;

public class Trophy : MonoBehaviour
{
    private Health health;
    private Damage damage;
    
    public int trophyLevel = 0;

    void Awake()
    {
        health = GetComponent<Health>();
        damage = GetComponent<Damage>();
        health.SetMaxHealthMultiplier(1 + trophyLevel * 0.05f);
        damage.SetDamageMultiplier(1 + trophyLevel * 0.05f);
    }

    [RPC]
    public void IncTrophyLevel()
    {
        if (networkView.isMine)
            trophyLevel++;

        if (trophyLevel < 4)
        {
            health.SetMaxHealthMultiplier(1 + trophyLevel * 0.05f);
            damage.SetDamageMultiplier(1 + trophyLevel * 0.05f);
        }
    }
}