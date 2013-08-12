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
        MinionManager minionManager = GameObject.FindGameObjectWithTag(Tags.minionManager).GetComponent<MinionManager>();

        XmlDocument document = new XMLReader(dataPath).GetXML();

        gameController.FirstMinionSpawn = float.Parse(document.GetElementsByTagName("FirstMinionspawn")[0].InnerText);
        gameController.SpawnTime = float.Parse(document.GetElementsByTagName("Minionspawn")[0].InnerText);
        minionManager.SpawnJitter = float.Parse(document.GetElementsByTagName("SpawnJitter")[0].InnerText);
        minionManager.MinionsPerPlayer = int.Parse(new XMLReader("Minion.xml").GetXML().GetElementsByTagName("countPerPlayer")[0].InnerText);
        minionManager.Init();
    }
}
