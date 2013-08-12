using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class MinionHealthbar : MonoBehaviour
{
    private float maxHealth = 0;
    private float curHealth = 0;
    private Health health;
    private GameObject lifeBar;
    
    //private float maxScaleZ;
    //private Vector3 leftTotal;
    //private Vector3 left;
    //private Vector3 right;

    // Use this for initialization
    void Start()
    {
        health = GetComponent<Health>();
        Object healthbar = Resources.Load("healthbar");
        lifeBar = (GameObject)Instantiate(healthbar, new Vector3(0f, 0f, 0f), Quaternion.identity);
        
        
        //maxScaleZ = lifeBar.transform.FindChild("healthbar").localScale.z;
        //leftTotal = lifeBar.transform.FindChild("left").position;
        //left = lifeBar.transform.FindChild("healthbar").FindChild("left").position;
        //right = lifeBar.transform.FindChild("healthbar").FindChild("right").position;
    }

    // Update is called once per frame
    void Update()
    {
        curHealth = health.HealthPoints;
        maxHealth = health.MaxHealth;
        if (curHealth / maxHealth == 1f)
        {
            lifeBar.SetActive(false);
            return;
        }
        if (curHealth <= 0)
        {
            Destroy(lifeBar);
            return;
        }
        lifeBar.SetActive(true);

        //lifeBar.transform.position = new Vector3(0f, 1.8f, 0f) + transform.position;
        //lifeBar.transform.LookAt(Camera.main.transform);
        //curHealth = health.HealthPoints;
        //maxHealth = health.MaxHealth;
        //lifeBar.transform.FindChild("healthbar").localScale = new Vector3(lifeBar.transform.FindChild("healthbar").localScale.x,
        //                                                                    lifeBar.transform.FindChild("healthbar").localScale.y,
        //                                                                    maxScaleZ * curHealth / maxHealth);
        //float healthbarLength = Mathf.Abs((right - left).magnitude);
        //Debug.Log(healthbarLength);
        //lifeBar.transform.FindChild("healthbar").position = new Vector3(lifeBar.transform.FindChild("healthbar").position.x,
        //    lifeBar.transform.FindChild("healthbar").position.y, leftTotal.z + healthbarLength / 2);
    }
}