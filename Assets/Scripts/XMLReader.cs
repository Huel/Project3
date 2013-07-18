using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class XMLReader
{
    private ArrayList _skillArray;
    private ArrayList _skillParameters;
    private XmlDocument xmlDoc;

    public XMLReader(string path)
    {
        xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(File.ReadAllText(path));
        ArrayList content = new ArrayList();

        foreach (XmlNode node in xmlDoc.ChildNodes)
            content.Add(new ArrayList{node});
        //XmlNodeList skills = xmlDoc.GetElementsByTagName("skill");//skills in an array

        //foreach (XmlNode skill in skills)
        //{
         //   XmlNodeList skillComponent = skill.ChildNodes;//all components of the current skill
         //   _skillParameters = new ArrayList();
         //   foreach (XmlNode skillParameters in skillComponent)//each component has two relevant values we want: Name and InnerText
         //   {
         //       _skillParameters.Add(skillParameters.Name);
        //        _skillParameters.Add(skillParameters.InnerText);
        //    }
        //    _skillArray.Add(_skillParameters);//each _skillParameters is one entire skill
        //}


        //foreach(ArrayList a in _skillArray)
        //{
        //    for (int i = 0; i < a.Count; i+=2 )
        //    {
        //        Debug.Log(a[i]+": "+a[i+1]);
        //    }
        //}
    }

    public string text()
    {
        return xmlDoc.InnerText;
    }

    public ArrayList skillArray //getter
    {
        get
        {
            return _skillArray;
        }
    }
}
