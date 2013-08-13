using UnityEditor;


[CustomEditor(typeof(ThirdPersonCamera))]
public class ThirdPersonCameraInspector : Editor
{
    private string[] states = new string[]{"Behind","Target","Free"};
    public override void OnInspectorGUI()
    {
        ThirdPersonCamera component = (ThirdPersonCamera)target;
        EditorGUILayout.LabelField("Camera State: " + states[(int)component.CamState]);
    }
}