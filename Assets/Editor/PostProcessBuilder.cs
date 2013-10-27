using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class PojectBuilder : MonoBehaviour
{
    //Unity menu item with Shortcut Cntrl+Space
    [MenuItem("Trash Clash/Build % ")]
    public static void BuildGame()
    {
        //Only continue if the user could save the scene
        if (!EditorApplication.SaveCurrentSceneIfUserWantsTo())
            return;

        //The build should be created in the right directory
        string path = Application.dataPath + "/../build/";
        //Save the IP-address to the NetworkSettings.xml (for local testing)
        SaveIPToXML("NetworkSettings.xml");

        //Find all scene-files
        string[] scenes = Directory.GetFiles(Application.dataPath + "/Scenes", "*.unity");
        //Create Build
        BuildPipeline.BuildPlayer(scenes, path + "Trash Clash.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        //Copy the XML-directory to the build directory
        FileUtil.CopyFileOrDirectory(Application.dataPath + "/XML", path + "Trash Clash_Data/XML");

        //Remove all meta data in the build directory
        string[] filePaths = Directory.GetFiles(path + "Trash Clash_Data/XML", "*.meta");
        foreach (var filePath in filePaths)
            File.Delete(filePath);


        //Run the build
        Process proc = new Process();
        proc.StartInfo.FileName = path + "Trash Clash.exe";
        proc.Start();

        //Open first scene and start the editor, that's why the current scene should be saved
        EditorApplication.OpenScene(scenes[0]);
        EditorApplication.isPlaying = true;
    }

    //Unity menu item with Shortcut Cntrl+W
    //Easily switch to our level
    [MenuItem("Trash Clash/Switch to Level %w")]
    public static void SwitchScene2()
    {
        if (!EditorApplication.SaveCurrentSceneIfUserWantsTo())
            return;
        string[] scenes = Directory.GetFiles(Application.dataPath + "/Scenes", "*.unity");

        EditorApplication.OpenScene(scenes[1]);
    }

    //Unity menu item with Shortcut Cntrl+E
    //Easily switch to our main menu scene
    [MenuItem("Trash Clash/Switch to Lobby %e")]
    public static void SwitchScene1()
    {
        if (!EditorApplication.SaveCurrentSceneIfUserWantsTo())
            return;
        string[] scenes = Directory.GetFiles(Application.dataPath + "/Scenes", "*.unity");
        EditorApplication.OpenScene(scenes[0]);
    }

    private static void SaveIPToXML(string dataPath)
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
        document.Save(Application.dataPath + "/XML/" + dataPath);
    }

    //Jump to scene when stopping the play mode
    public void OnApplicationQuit()
    {
        string[] scenes = Directory.GetFiles(Application.dataPath + "/Scenes", "*.unity");
        EditorApplication.OpenScene(scenes[1]);
    }
}