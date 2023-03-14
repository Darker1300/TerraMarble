using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// v1.2 - manager class for the Mecha scene
public class MechaSampleGame_Manager : MonoBehaviour
{
    Camera mainCamera;
    float camFollowVelX = 2;
    float camFollowVelY = 2f;

    public SpriteRenderer indicator;
    public Text textField;
    public Image textImage;

    public List<MechaSampleGame_Char> charsList = new List<MechaSampleGame_Char>();
    MechaSampleGame_Char currentChar;
    public MechaSampleGame_Char CurrentChar
    {
        get => currentChar;
        set
        {
            currentChar = value;
            textField.text = currentChar.DescriptionText;
            textImage.color = currentChar.ColorTextImage;
        }
    }

    void Awake()
    {
        UpdateCharsList();
        mainCamera = Camera.main;
    }

    void Start()
    {
        Physics2D.gravity = new Vector2(0, -40);
        CurrentChar = FindObjectOfType<MechaSampleGame_Player>();
    }

    void Update()
    {
        indicator.transform.position = CurrentChar.mainAxialBone.transform.position + new Vector3(0, CurrentChar.indicatorDistance);

        if (Input.GetKeyDown(KeyCode.Tab) && CurrentChar.mainAxialBone.isGrounded && !CurrentChar.mainAxialBone.isJumping)
        {
            SwitchCurrentChar();
        }

        if (CurrentChar.mainAxialBone.isJumping)
        {
            CurrentChar.playerController.bodyAcceleration = 15;
        }
        else
        {
            CurrentChar.playerController.bodyAcceleration = 5;
        }

        Vector3 camPosition = mainCamera.transform.position;
        camPosition.x = Mathf.Lerp(camPosition.x, CurrentChar.mainAxialBone.transform.position.x, Time.deltaTime * camFollowVelX);
        camPosition.y = Mathf.Lerp(camPosition.y, CurrentChar.mainAxialBone.transform.position.y, Time.deltaTime * camFollowVelY);
        mainCamera.transform.position = camPosition;
    }

    void UpdateCharsList()
    {
        charsList.Clear();
        charsList.AddRange(FindObjectsOfType<MechaSampleGame_Char>());
    }

    public void SwitchCurrentChar()
    {
        int idx = charsList.IndexOf(CurrentChar);

        // revoke current char
        RevokeCurrentChar();

        idx++;
        if (idx >= charsList.Count)
            idx = 0;

        // promote to current char
        PromoteToCurrentChar(charsList[idx]);
    }

    public void RevokeCurrentChar()
    {
        CurrentChar.playerController.positionTarget.SetParent(CurrentChar.mainAxialBone.transform);
        CurrentChar.mainAxialBone.playerControllerType = PL2D_AxialBone.PlayerControllerEnum.PathFollow;
        if (CurrentChar is MechaSampleGame_Enemy)
        {
            (CurrentChar as MechaSampleGame_Enemy).LoadClosestPath();
        }
        else
        {
            CurrentChar.playerController.pathPoints[0] = CurrentChar.mainAxialBone.transform.position;
            CurrentChar.playerController.enableFlipToMouse = false;
        }
    }

    public void PromoteToCurrentChar(MechaSampleGame_Char charr)
    {
        CurrentChar = charr;

        CurrentChar.playerController.positionTarget.SetParent(CurrentChar.animator.transform);
        CurrentChar.mainAxialBone.playerControllerEnabled = true;
        CurrentChar.mainAxialBone.playerControllerType = PL2D_AxialBone.PlayerControllerEnum.KeyboardInput;
        CurrentChar.playerController.positionTarget.position = CurrentChar.mainAxialBone.transform.position;

        if (CurrentChar is MechaSampleGame_Enemy)
        {

        }
        else
        {
            CurrentChar.playerController.pathPoints[0] = CurrentChar.mainAxialBone.transform.position;
            CurrentChar.playerController.enableFlipToMouse = true;
        }
    }
}
