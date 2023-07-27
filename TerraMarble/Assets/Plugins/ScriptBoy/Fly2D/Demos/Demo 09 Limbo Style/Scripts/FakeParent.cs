using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptBoy.Fly2D.Demo
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(90000)]
    public class FakeParent : MonoBehaviour
    {
        public Transform parent;
        public Vector3 localPosition;

        void LateUpdate()
        {
            transform.position = parent.TransformPoint(localPosition);
        }
    }
}
