using System.Xml;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public void Init()
    {
        getDataFromXML("GameSettings.xml");
    }
    
    private void getDataFromXML(string dataPath)
    {
        GameController gameController = GetComponent<GameController>();
        LocalPlayerController localPlayerController = GameObject.FindGameObjectWithTag(Tags.localPlayerController).GetComponent<LocalPlayerController>();
               
        XmlDocument document = new XMLReader(dataPath).GetXML();

        gameController.FirstMinionSpawn = float.Parse(document.GetElementsByTagName("FirstMinionspawn")[0].InnerText);
        gameController.SpawnTime = float.Parse(document.GetElementsByTagName("Minionspawn")[0].InnerText);

        for (int i = 1; i < 4; i++)
            localPlayerController.setMinionDeployment(i, int.Parse(document.GetElementsByTagName("Lane"+i)[0].InnerText));

        localPlayerController.spawnJitter = float.Parse(document.GetElementsByTagName("SpawnJitter")[0].InnerText);
    }
}
