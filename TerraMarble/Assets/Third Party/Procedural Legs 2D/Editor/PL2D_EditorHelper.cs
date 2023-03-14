/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using UnityEditor;
using UnityEngine;

public static class PL2D_EditorHelper
{
    public static void HorizontalSeparator()
    {
        EditorGUILayout.Space();
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x + 10, rect.y), new Vector2(rect.width, rect.y));
        EditorGUILayout.EndHorizontal();
    }

    public static void HorizontalSeparatorSpace()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.width, rect.y));
        EditorGUILayout.EndHorizontal();
    }

    public static void HorizontalSeparatorBold()
    {
        EditorGUILayout.Space();
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = new Color(0.1f, 0.1f, 0.1f);
        Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.width, rect.y));
        EditorGUILayout.EndHorizontal();
        Handles.color = Color.gray;
    }

    public static void LabelTab1(string label)
    {
        GUIStyle style = GUI.skin.box;
        GUIContent content = new GUIContent(label);
        int labelSize = (int)style.CalcSize(content).x + 10;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = new Color(0.1f, 0.1f, 0.1f);
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.width + 15, rect.y));
        float _rectWidth = rect.width;
        for (int i = 1; i <= 9; i++)
        {
            Handles.DrawLine(new Vector2(rect.x + labelSize + i, rect.y + i), new Vector2(_rectWidth + 15, rect.y + i));
        }

        EditorGUILayout.EndHorizontal();
        Handles.color = Color.gray;
    }

    public static void BeginVerticalHeader1(string label)
    {
        PL2D_EditorHelper.LabelTab1(label);

        var rect = EditorGUILayout.BeginVertical();
        Handles.color = new Color(0.1f, 0.1f, 0.1f);
        Handles.DrawLine(new Vector2(rect.xMin, rect.yMin - 19), new Vector2(rect.xMin - 10, rect.yMin - 9));
        Handles.DrawLine(new Vector2(rect.xMin - 10, rect.yMin - 9), new Vector2(rect.xMin - 10, rect.yMax));
        Handles.DrawLine(new Vector2(rect.xMin - 9, rect.yMin - 10), new Vector2(rect.xMin - 9, rect.yMax));
        Handles.color = Color.gray;
    }

    public static bool BeginVerticalHeader1Foldout(string label_, bool show)
    {
        string label = "   " + label_;
        PL2D_EditorHelper.LabelTab1(label);

        var rect = EditorGUILayout.BeginVertical();
        Handles.color = new Color(0.1f, 0.1f, 0.1f);

        GUI.BeginGroup(new Rect(new Vector2(10, -18), new Vector2(15, rect.yMax)));
        GUIStyle s = EditorStyles.foldout;
        bool result = EditorGUILayout.Foldout(show, "", s);
        GUI.EndGroup();

        Handles.DrawLine(new Vector2(rect.xMin, rect.yMin - 19), new Vector2(rect.xMin - 10, rect.yMin - 9));
        Handles.DrawLine(new Vector2(rect.xMin - 10, rect.yMin - 9), new Vector2(rect.xMin - 10, rect.yMax));
        Handles.DrawLine(new Vector2(rect.xMin - 9, rect.yMin - 10), new Vector2(rect.xMin - 9, rect.yMax));
        Handles.color = Color.gray;

        return result;
    }

    public static void LabelTab2(string label)
    {
        Handles.color = new Color(0.2f, 0.2f, 0.2f);
        GUIContent content = new GUIContent(label);
        GUIStyle style = GUI.skin.box;
        int labelSize = (int)style.CalcSize(content).x + 10;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        var rect = EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.width + 15, rect.y));
        float _rectWidth = rect.width;
        for (int i = 1; i <= 7; i++)
        {
            Handles.DrawLine(new Vector2(rect.x + labelSize + i, rect.y + i), new Vector2(_rectWidth + 15, rect.y + i));
        }

        EditorGUILayout.EndHorizontal();
        Handles.color = Color.gray;
    }

    public static void BeginVerticalHeader2(string label)
    {
        PL2D_EditorHelper.LabelTab2(label);

        var rect = EditorGUILayout.BeginVertical();
        Handles.color = new Color(0.2f, 0.2f, 0.2f);
        Handles.DrawLine(new Vector2(rect.xMin + 4, rect.yMin - 19), new Vector2(rect.xMin - 5, rect.yMin - 10));
        Handles.DrawLine(new Vector2(rect.xMin - 5, rect.yMin - 10), new Vector2(rect.xMin - 5, rect.yMax));
        Handles.DrawLine(new Vector2(rect.xMin - 4, rect.yMin - 11), new Vector2(rect.xMin - 4, rect.yMax));
        Handles.color = Color.gray;
    }

    public static bool BeginVerticalHeader2Foldout(string label_, bool show)
    {
        string label = "  " + label_;
        PL2D_EditorHelper.LabelTab2(label);

        var rect = EditorGUILayout.BeginVertical();
        Handles.color = new Color(0.2f, 0.2f, 0.2f);

        GUI.BeginGroup(new Rect(new Vector2(10, -18), new Vector2(15, rect.yMax)));
        GUIStyle s = EditorStyles.foldout;
        bool result = EditorGUILayout.Foldout(show, "", s);
        GUI.EndGroup();

        Handles.DrawLine(new Vector2(rect.xMin + 4, rect.yMin - 19), new Vector2(rect.xMin - 5, rect.yMin - 10));
        Handles.DrawLine(new Vector2(rect.xMin - 5, rect.yMin - 10), new Vector2(rect.xMin - 5, rect.yMax));
        Handles.DrawLine(new Vector2(rect.xMin - 4, rect.yMin - 11), new Vector2(rect.xMin - 4, rect.yMax));
        Handles.color = Color.gray;

        return result;
    }

    public static void LabelTab3(string label)
    {
        Handles.color = new Color(0.3f, 0.3f, 0.3f);
        GUIContent content = new GUIContent(label);
        GUIStyle style = GUI.skin.box;
        int labelSize = (int)style.CalcSize(content).x + 10;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        var rect = EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.width + 15, rect.y));
        float _rectWidth = rect.width;
        for (int i = 1; i <= 5; i++)
        {
            Handles.DrawLine(new Vector2(rect.x + labelSize + i, rect.y + i), new Vector2(_rectWidth + 15, rect.y + i));
        }
        EditorGUILayout.EndHorizontal();
        Handles.color = Color.gray;
    }

    public static void BeginVerticalHeader3(string label)
    {
        PL2D_EditorHelper.LabelTab3(label);

        var rect = EditorGUILayout.BeginVertical();
        Handles.color = new Color(0.3f, 0.3f, 0.3f);
        Handles.DrawLine(new Vector2(rect.xMin + 4, rect.yMin - 19), new Vector2(rect.xMin - 0, rect.yMin - 15));
        Handles.DrawLine(new Vector2(rect.xMin - 0, rect.yMin - 15), new Vector2(rect.xMin - 0, rect.yMax));
        Handles.DrawLine(new Vector2(rect.xMin + 1, rect.yMin - 16), new Vector2(rect.xMin + 1, rect.yMax));
        Handles.color = Color.gray;
    }

    public static bool BeginVerticalHeader3Foldout(string label_, bool show)
    {
        string label = "  " + label_;
        PL2D_EditorHelper.LabelTab3(label);

        var rect = EditorGUILayout.BeginVertical();
        Handles.color = new Color(0.3f, 0.3f, 0.3f);

        GUI.BeginGroup(new Rect(new Vector2(10, -18), new Vector2(15, rect.yMax)));
        GUIStyle s = EditorStyles.foldout;
        bool result = EditorGUILayout.Foldout(show, "", s);
        GUI.EndGroup();

        Handles.DrawLine(new Vector2(rect.xMin + 4, rect.yMin - 19), new Vector2(rect.xMin - 0, rect.yMin - 15));
        Handles.DrawLine(new Vector2(rect.xMin - 0, rect.yMin - 15), new Vector2(rect.xMin - 0, rect.yMax));
        Handles.DrawLine(new Vector2(rect.xMin + 1, rect.yMin - 16), new Vector2(rect.xMin + 1, rect.yMax));
        Handles.color = Color.gray;

        return result;
    }

    public static void VerticalSeparator()
    {
        var rect = EditorGUILayout.BeginVertical();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x, rect.yMin));
        EditorGUILayout.EndVertical();
    }

    public static void LabelBold(string label)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
    }

}