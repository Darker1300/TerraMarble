using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptBoy.Fly2D.Demo
{
    public class Player : MonoBehaviour
    {
        private CharacterController2D m_CharacterController;
        private Animator m_Animator;

        //Move speed of the character
        public float moveSpeed;

        private void Start()
        {
            //Get the Animator attached to the GameObject
            m_Animator = GetComponent<Animator>();
            //Get the CharacterController2D attached to the GameObject
            m_CharacterController = GetComponent<CharacterController2D>();
        }

        private void Update()
        {
            //Translate the left and right button presses or the horizontal joystick movements to a float
            float horizontalAxis = Input.GetAxis("Horizontal");
            //Move the character based on the horizontal axis
            m_CharacterController.Move(horizontalAxis * moveSpeed);
            //Sends the value from the horizontal axis input to the animator. Change the settings in the
            //Animator to define when the character is running
            m_Animator.SetBool("Run", Mathf.Abs(horizontalAxis) != 0);
        }
    }
}