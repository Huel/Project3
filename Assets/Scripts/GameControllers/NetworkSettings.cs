using System.Net;
using System.Xml;
using UnityEngine;

public class NetworkSettings : MonoBehaviour 
{
    public void Init()
    {
        
        getDataFromXML("NetworkSettings.xml");
    }

    private void getDataFromXML(string dataPath)
    {
        NetworkManager networkManager = GetComponent<NetworkManager>();
        XmlDocument document = new XMLReader(dataPath).GetXML();
        networkManager.setIP(document.GetElementsByTagName("IP")[0].InnerText);
    }

    //private void saveIPXML(string dataPath)
    //{
    //    XmlDocument document = new XMLReader(dataPath).GetXML();
    //    XmlElement element = document.DocumentElement;
        
    //    string strHostName = System.Net.Dns.GetHostName();
    //    IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

    //    IPAddress[] addr = ipEntry.AddressList;
    //    element.RemoveAll();
    //    XmlElement ipElement = document.CreateElement("IP");
    //    ipElement.InnerText = addr[addr.Length - 1].ToString();

    //    element.AppendChild(ipElement);
    //    document.Save(Application.dataPath + "/XML/" + dataPath);
    //}
}
