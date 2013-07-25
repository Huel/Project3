using System.IO;
using System.Xml;

public class XMLReader
{

    private XmlDocument document = new XmlDocument();

    public XMLReader(string path)
    {
        try
        {
            document.LoadXml(File.ReadAllText(Path.GetFullPath("Assets/XML/" + path)));
            /*foreach (XmlNode node in document.ChildNodes)
            switch(node.GetType().ToString())
            {
                case "System.Xml.XmlElement":
                        content.Add(new ArrayList{node.Name, GetInnerChilds((XmlElement)node)});
                        break;
                case "System.Xml.XmlText":
                        content.Add(GetInnerChilds((XmlText)node));
                        break;
            }
            */
        }
        catch (IOException error)
        {
            throw (error);
        }
    }

    /*
    ArrayList GetInnerChilds(XmlElement innerNode)
    {
        ArrayList list = new ArrayList();
        foreach (XmlNode node in innerNode.ChildNodes)
            switch(node.GetType().ToString())
            {
                case "System.Xml.XmlElement":
                {
                    list.Add(new ArrayList{node.Name, GetInnerChilds((XmlElement)node)});
                    break;
                }
                case "System.Xml.XmlText":
                    list.Add(GetInnerChilds((XmlText)node));
                    break;
            }
        return list;
    }
	
    string GetInnerChilds(XmlText innerText)
    {
        return innerText.Value;
    }
    */

    public XmlDocument GetXML()
    {
        return document;
    }
}
