using UnityEditor;


[CustomEditor(typeof(Speed))]
public class SpeedInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Speed component = (Speed)target;
        EditorGUILayout.LabelField("Current Speed: " + component.CurrentSpeed);
        EditorGUILayout.LabelField("Stamina: " + component.Stamina + "/" + component.MaxStamina);
        EditorGUILayout.LabelField("Stamina Decay: " + component.StaminaDecay + "/s");
        EditorGUILayout.LabelField("Stamina Regeneration: " + component.StaminaRegenaration + "/s");
        EditorGUILayout.LabelField("Min Stamina: " + component.MinStamina);
    }
}