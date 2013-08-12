using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class MinionHealthbar : MonoBehaviour
{
    private float maxHealth = 0;
    private float curHealth = 0;
    private Health health;
    private UISprite healthbar;
    
    void Start()
    {
        health = GetComponent<Health>();
        healthbar = transform.FindChild("sprite_healthbar").GetComponent<UISprite>();
    }

    // Update is called once per frame
    void Update()
    {
        curHealth = health.HealthPoints;
        maxHealth = health.MaxHealth;

        if (curHealth / maxHealth == 1f)
        {
            healthbar.alpha = 0;
            return;
        }
        if (curHealth <= 0)
        {
            healthbar.alpha = 0;
            return;
        }
        healthbar.transform.LookAt(Camera.main.transform);
        healthbar.alpha = 1;
        healthbar.fillAmount = curHealth/maxHealth;
    }
}