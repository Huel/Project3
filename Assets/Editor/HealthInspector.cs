using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Health))]
public class HealthInspector : Editor
{
    private bool showInputSettings = true;
    private bool showJSONContent = false;
    public override void OnInspectorGUI()
    {
        Health component = (Health)target;
        EditorGUILayout.LabelField("Health: " + component.HealthPoints + "/" + component.MaxHealth);
        EditorGUILayout.LabelField("MinHealth: " + component.MinHealth);
    }
}