/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using UnityEditor;

[CustomEditor(typeof(PL2D_PlayerController))]
public class PL2D_PlayerController_Editor : Editor
{
    PL2D_PlayerController pl2dPlayerController;

    void OnEnable()
    {
        pl2dPlayerController = (PL2D_PlayerController)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Use the PL2D Animator or the API to edit", MessageType.Info);
    }
}
