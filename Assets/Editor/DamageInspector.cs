using UnityEditor;


[CustomEditor(typeof(Damage))]
public class DamageInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Damage component = (Damage)target;
        EditorGUILayout.LabelField("Damage: " + component.CurrentDamage);
        EditorGUILayout.LabelField("Hit Speed: " + component.HitSpeed + "s");
    }
}