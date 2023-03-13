/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using UnityEditor;

[CustomEditor(typeof(PL2D_Leg))]
public class PL2D_Leg_Editor : Editor
{
    PL2D_Leg pl2dLeg;

    void OnEnable()
    {
        pl2dLeg = (PL2D_Leg)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Use the PL2D Animator or the API to edit", MessageType.Info);
    }
}
