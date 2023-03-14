/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using UnityEditor;

[CustomEditor(typeof(PL2D_AngleSolver))]
public class PL2D_AngleSolver_Editor : Editor
{
    PL2D_AngleSolver pl2dAngleSolver;

    void OnEnable()
    {
        pl2dAngleSolver = (PL2D_AngleSolver)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Use the PL2D Animator or the API to edit", MessageType.Info);
    }
}
