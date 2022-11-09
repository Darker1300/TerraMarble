
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace z
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class TwoButtonDragComposite : InputBindingComposite<Vector2>
    {
        static TwoButtonDragComposite()
        {
            InputSystem.RegisterBindingComposite<TwoButtonDragComposite>();
        }

        [RuntimeInitializeOnLoadMethod]
        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private static void Init()
        {
        }

        public override Vector2 ReadValue(ref InputBindingCompositeContext context)
        {
            var b1 = context.ReadValueAsButton(Button1);
            var b2 = context.ReadValueAsButton(Button2);
            var x = context.ReadValue<float>(Axis1);
            var y = context.ReadValue<float>(Axis2);
            var v = new Vector2(x, y);

            return b1 && b2 && v.magnitude > 0.0f ? v : default;
        }

        public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
        {
            return ReadValue(ref context).magnitude;
        }

        #region Fields

        [InputControl(layout = "Button")]
        [UsedImplicitly]
        public int Button1;

        [InputControl(layout = "Button")]
        [UsedImplicitly]
        public int Button2;

        [InputControl(layout = "Axis")]
        [UsedImplicitly]
        public int Axis1;

        [InputControl(layout = "Axis")]
        [UsedImplicitly]
        public int Axis2;

        #endregion
    }
}
