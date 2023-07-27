using UnityEngine;
using System;
using System.Reflection;

namespace ScriptBoy.Fly2D.Demo
{
    [ExecuteInEditMode]
    public class CheckDependencies : MonoBehaviour
    {
        [SerializeField] private string[] m_Dependencies;

        private void Start()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var type = Type.GetType("ScriptBoy.Fly2D.DependencySolver,ScriptBoy.Fly2D.Editor");
                var CheckMethod = type.GetMethod("Check");
                CheckMethod.Invoke(null, new object[] { m_Dependencies});
            }
#endif
        }
    }
}