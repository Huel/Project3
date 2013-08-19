using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class MinionHealthbar : MonoBehaviour
{
    private float maxHealth = 0;
    private float curHealth = 0;
    private Health health;
    private UISprite healthbar;
    private UISprite healthbarF;
    private UISprite healthbarBG;

    private GameObject healthObj;
    private GameObject healthObjFront;
    private GameObject healthObjBG;
    
    void Start()
    {
        health = GetComponent<Health>();
        healthObj = transform.FindChild("sprite_healthbar").gameObject;
        healthObjFront = transform.FindChild("sprite_healthbar_front").gameObject;
        healthObjBG = transform.FindChild("sprite_healthbar_bg").gameObject;
        healthbar = healthObj.GetComponent<UISprite>();
        healthbarF = healthObjFront.GetComponent<UISprite>();
        healthbarBG = healthObjBG.GetComponent<UISprite>();

        if (GetComponent<Team>().ID == Team.TeamIdentifier.Team1)
            healthObj.GetComponent<UISprite>().color = new Color(0, 1, 0);
        else  if (GetComponent<Team>().ID == Team.TeamIdentifier.Team2)
            healthObj.GetComponent<UISprite>().color = new Color(1, 1, 0);
        else
            healthObj.GetComponent<UISprite>().color = new Color(1, 1, 1);
    }
    // Update is called once per frame
    void Update()
    {
        curHealth = health.HealthPoints;
        maxHealth = health.MaxHealth;

        if (curHealth / maxHealth == 1f || curHealth <= 0)
        {
            healthObj.SetActive(false);
            healthObjFront.SetActive(false);
            healthObjBG.SetActive(false);
            return;
        }
        healthObj.SetActive(true);
        healthObjFront.SetActive(true);
        healthObjBG.SetActive(true);

        healthbar.transform.LookAt(Camera.main.transform);
        healthbarF.transform.LookAt(Camera.main.transform);
        healthbarBG.transform.LookAt(Camera.main.transform);

        healthbar.alpha = 1;
        healthbar.fillAmount = curHealth/maxHealth;
    }
}