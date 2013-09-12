using UnityEditor;


[CustomEditor(typeof(OrbitCamera))]
public class OrbitCameraInspector : Editor
{
    private string[] states = new string[]{"Behind","Target","Free"};
    public override void OnInspectorGUI()
    {
        OrbitCamera component = (OrbitCamera)target;
        EditorGUILayout.LabelField("Camera State: " + states[(int)component.CamState]);
    }
}