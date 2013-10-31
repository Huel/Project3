using UnityEngine;
using System.Collections;

public class TextBackground : MonoBehaviour
{
    public dfLabel text;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (text.Text == "")
            GetComponent<dfControl>().Hide();
        else
            GetComponent<dfControl>().Show();
	}
}
