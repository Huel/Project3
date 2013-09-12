using UnityEditor;


[CustomEditor(typeof(Valve))]
public class ValveInspector : Editor
{
    private bool _localFoldOut = false;
    public override void OnInspectorGUI()
    {
        Valve component = (Valve)target;
        EditorGUILayout.LabelField("Valve Team: " + component.GetComponent<Team>().ID);
        EditorGUILayout.LabelField("State: " + component.ValveState + " (" + component.State+"%)");
        if (component.ValveState != ValveStates.NotOccupied && component.ValveState != ValveStates.Opened && component.ValveState != ValveStates.Closed)
        {
            EditorGUILayout.LabelField("Occupant: " + component.Occupant + " ("+component.MinionCount+" Minions with a productivity of "+component.Productivity+")");

        }
        _localFoldOut = EditorGUILayout.Foldout(_localFoldOut, "Local Data");
        if (_localFoldOut)
        {
            EditorGUILayout.LabelField("   Player ID: " + component.LocalPlayerID);
            if (component.LocalMinionCount > 0)
            {
                EditorGUILayout.LabelField("   " + component.LocalMinionCount + " Minions with a productivity of " + component.LocalProductivity);
                foreach (MinionAgent localMinion in component.LocalMinions)
                {
                    EditorGUILayout.ObjectField("   ", localMinion, typeof(MinionAgent), true);
                }
            }

        }
    }
}