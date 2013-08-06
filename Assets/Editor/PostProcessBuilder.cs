using System.IO;
using System.Net;
using System.Xml;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;

public class PostProcessBuilder : MonoBehaviour
{
    [MenuItem("Build Project/Build")]
    public static void BuildGame()
    {
        // Get filename.
        string path = Application.dataPath + "/../build/";
        

        // Build player.
        string[] scenes = Directory.GetFiles(Application.dataPath + "/Scenes", "*.unity");
        BuildPipeline.BuildPlayer(scenes, path + "Project3.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory(Application.dataPath + "/XML", path + "Project3_Data/XML");

        string[] filePaths = Directory.GetFiles(path + "Project3_Data/XML", "*.meta");
        foreach (var filePath in filePaths)
        {
            File.Delete(filePath);
        }
        // save IP
        saveIPXML("NetworkSettings.xml");

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "Project3.exe";
        proc.Start();
    }

    private static void saveIPXML(string dataPath)
    {
        XmlDocument document = new XMLReader(dataPath).GetXML();
        XmlElement element = document.DocumentElement;

        string strHostName = System.Net.Dns.GetHostName();
        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

        IPAddress[] addr = ipEntry.AddressList;
        element.RemoveAll();
        XmlElement ipElement = document.CreateElement("IP");
        ipElement.InnerText = addr[addr.Length - 1].ToString();

        element.AppendChild(ipElement);
        document.Save(Application.dataPath + "/../build/Project3_Data/XML/" + dataPath);
    }
}