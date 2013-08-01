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

        networkManager.ip = document.GetElementsByTagName("IP")[0].InnerText;
    }
}
