using UnityEditor;


[CustomEditor(typeof(Health))]
public class HealthInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Health component = (Health)target;
        EditorGUILayout.LabelField("Health: " + component.HealthPoints + "/" + component.MaxHealth);
        EditorGUILayout.LabelField("MinHealth: " + component.MinHealth);
    }
}