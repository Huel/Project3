using System.IO;
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

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "Project3.exe";
        proc.Start();
    }
}