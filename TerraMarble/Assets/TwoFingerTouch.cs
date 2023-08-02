using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TwoFingerTouch : MonoBehaviour
{
    private bool isLeftTouched = false;
    private bool isRightTouched = false;

    void Update()
    {
        CheckTouchInput();
    }

    private void CheckTouchInput()
    {
        if (Touchscreen.current == null)
        {
            // InputSystem is not supported on the current platform
            return;
        }

        // Check if any touch is registered on the screen
        if (Touchscreen.current.touches.Count > 0)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                // Check if the touch position is on the left half of the screen
                if (touch.position.x.ReadValue() < Screen.width / 2)
                {
                    isLeftTouched = true;
                }
                // Check if the touch position is on the right half of the screen
                else
                {
                    isRightTouched = true;
                }
            }
        }
        else
        {
            // No touch on the screen, reset both flags
            isLeftTouched = false;
            isRightTouched = false;
        }

        // Check if both sides are being touched
        if (isLeftTouched && isRightTouched)
        {
            // Both sides are being touched, do something here
            Debug.Log("Both left and right sides are touched!");
        }

    }
}