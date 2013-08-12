using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class MinionHealthbar : MonoBehaviour
{
    private float maxHealth = 0;
    private float curHealth = 0;
    private Health health;
    private UISprite healthbar;
    private GameObject healthObj;
    
    void Start()
    {
        health = GetComponent<Health>();
        healthObj = transform.FindChild("sprite_healthbar").gameObject;
        healthbar = healthObj.GetComponent<UISprite>();
        
    }

    // Update is called once per frame
    void Update()
    {
        curHealth = health.HealthPoints;
        maxHealth = health.MaxHealth;

        if (curHealth / maxHealth == 1f || curHealth <= 0)
        {
            healthObj.SetActive(false);
            return;
        }
        healthObj.SetActive(true);
        healthbar.transform.LookAt(Camera.main.transform);
        healthbar.alpha = 1;
        healthbar.fillAmount = curHealth/maxHealth;
    }
}