using UnityEngine;
using System.Collections;
using System.IO;

public class XML : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.AddComponent("Skill");
        gameObject.GetComponent<Skill>().Init("Basic Attack", gameObject);
        //new Skill("Basic Attack", gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
