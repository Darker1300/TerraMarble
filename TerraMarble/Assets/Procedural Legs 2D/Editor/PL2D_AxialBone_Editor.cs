/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using UnityEditor;

[CustomEditor(typeof(PL2D_AxialBone))]
public class PL2D_AxialBone_Editor : Editor
{
    PL2D_AxialBone pl2dAxialBone;

    void OnEnable()
    {
        pl2dAxialBone = (PL2D_AxialBone)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Use the PL2D Animator or the API to edit", MessageType.Info);
    }
}
