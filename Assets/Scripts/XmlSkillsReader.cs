using UnityEngine;
using System.Collections;
using System.Xml;

public class XmlSkillsReader : MonoBehaviour 
{

    public TextAsset skillInformation;
    private ArrayList _skillArray;
    private ArrayList _skillParameters;

	void Start ()
    {
        _skillArray = new ArrayList();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(skillInformation.text);
        XmlNodeList skills = xmlDoc.GetElementsByTagName("skill");//skills in an array

        foreach (XmlNode skill in skills)
        {
            XmlNodeList skillComponent = skill.ChildNodes;//all components of the current skill
            _skillParameters = new ArrayList();
            foreach (XmlNode skillParameters in skillComponent)//each component has two relevant values we want: Name and InnerText
            {
                _skillParameters.Add(skillParameters.Name);
                _skillParameters.Add(skillParameters.InnerText);
            }
            _skillArray.Add(_skillParameters);//each _skillParameters is one entire skill
        }


        //foreach(ArrayList a in _skillArray)
        //{
        //    for (int i = 0; i < a.Count; i+=2 )
        //    {
        //        Debug.Log(a[i]+": "+a[i+1]);
        //    }
        //}
	}
	
	void Update () 
	{
	
	}

    public ArrayList skillArray //getter
    {
        get
        {
            return _skillArray;
        }
    }
}
