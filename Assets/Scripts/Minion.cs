using UnityEngine;
using System.Collections;
using System.Xml;

public class Minion : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		
	}

	private void ConvertXML()
	{
		XmlDocument document = new XMLReader("Minion.xml").GetXML();
		XmlElement health = null;
		foreach (XmlElement node in document.GetElementsByTagName("health"))
			health = node;

		if (health != null)
		{
			health.GetElementsByTagName("healthPoints");
		}
		
	}
}
