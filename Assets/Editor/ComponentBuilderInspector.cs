using UnityEditor;


[CustomEditor(typeof(ComponentBuilder))]
public class ComponentBuilderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        ComponentBuilder component = (ComponentBuilder)target;
        if (component.state == ComponentBuilder.LoadingState.InActive)
            component.xmlFile = EditorGUILayout.TextField("XML-Name:", component.xmlFile);
        else if (component.state == ComponentBuilder.LoadingState.Loaded)
        {
            EditorGUILayout.LabelField(component.xmlFile + ".XML successfully loaded.");

            if (component.healthParsed)
                EditorGUILayout.LabelField("+ health component loaded");
            if (component.speedParsed)
                EditorGUILayout.LabelField("+ speed component loaded");
            if (component.damageParsed)
                EditorGUILayout.LabelField("+ damage component loaded");
        }
        else
            EditorGUILayout.LabelField("Didn't load because networkView isn't mine.");

    }
}