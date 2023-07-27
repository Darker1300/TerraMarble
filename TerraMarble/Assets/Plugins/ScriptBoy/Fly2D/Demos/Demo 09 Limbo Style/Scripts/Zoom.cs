using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptBoy.Fly2D.Demo
{
    public class Zoom : MonoBehaviour
    {
        public new Camera camera;
        public float speed;

        private void Update()
        {
            camera.orthographicSize += Input.mouseScrollDelta.y * speed * Time.deltaTime;
        }
    }
}
