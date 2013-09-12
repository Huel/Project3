using System.IO;
using System.Xml;
using UnityEngine;

public class XMLReader
{

    private XmlDocument document = new XmlDocument();

    public XMLReader(string path)
    {
        try
        {
            document.LoadXml(File.ReadAllText(Application.dataPath + "/XML/" + path));
        }
        catch (IOException error)
        {
            throw (error);
        }
    }

    public XmlDocument GetXML()
    {
        return document;
    }
}
