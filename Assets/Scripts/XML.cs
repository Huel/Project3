using UnityEngine;
using System.Collections;
using System.IO;

public class XML : MonoBehaviour {

	// Use this for initialization
	void Start () {
        XMLReader test = new XMLReader(Path.GetFullPath("Assets/Scripts/XML/SkillsTemplate.xml"));
        Debug.Log(test.text());
        Debug.Log(Path.GetFullPath("Assets/Scripts/XML/SkillsTemplate.xml"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
