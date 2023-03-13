using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// v1.1.1 - "Octopus Like" body added
public class SampleSceneController : MonoBehaviour
{
    public Dropdown bodiesDropdown;
    public Dropdown animationsDropdown;

    public Text infoText;

    public List<Transform> bodies;

    LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        bodiesDropdown.options.Clear();
        foreach (Transform body in bodies)
        {
            bodiesDropdown.options.Add(new Dropdown.OptionData(body.name));
        }

        PL2D_Animator animator = bodies[bodiesDropdown.value].GetComponentInChildren<PL2D_Animator>();
        animationsDropdown.options.Clear();
        int _animationsPopupCount = animator.animationsSelectionPopup.Count;
        for (int i = 0; i < _animationsPopupCount - 1; i++)
        {
            animationsDropdown.options.Add(new Dropdown.OptionData(animator.animationsSelectionPopup[i]));
        }

        SelectBody();
    }

    void Update()
    {

    }

    PL2D_Animator selectedAnimator;

    public void SelectBody()
    {
        foreach (Transform body in bodies)
        {
            body.gameObject.SetActive(false);
        }

        bodies[bodiesDropdown.value].gameObject.SetActive(true);

        selectedAnimator = bodies[bodiesDropdown.value].GetComponentInChildren<PL2D_Animator>();

        foreach (PL2D_AxialBone axialBone in selectedAnimator.axialBones)
        {
            foreach (PL2D_Leg leg in axialBone.legs)
            {
                leg.isMoving = false;
            }
        }

        animationsDropdown.options.Clear();
        int _animationsPopupCount = selectedAnimator.animationsSelectionPopup.Count;
        for (int i = 0; i < _animationsPopupCount - 1; i++)
        {
            animationsDropdown.options.Add(new Dropdown.OptionData(selectedAnimator.animationsSelectionPopup[i]));
        }

        animationsDropdown.value = 0;

        bodiesDropdown.RefreshShownValue();
        animationsDropdown.RefreshShownValue();

        SelectAnimation();

        bodies[bodiesDropdown.value].position = new Vector3(-40, 0, 0);
    }

    public void SelectAnimation()
    {
        lineRenderer.positionCount = 0;

        PL2D_Animator animator = bodies[bodiesDropdown.value].GetComponentInChildren<PL2D_Animator>();
        animator.LoadAnimation(animationsDropdown.options[animationsDropdown.value].text);

        if (animator.axialBones[0].playerControllerType == PL2D_AxialBone.PlayerControllerEnum.KeyboardInput)
        {
            if (animator.axialBones[0].pl2d_PlayerController.enableJump)
            {
                infoText.text = "Use 'A' and 'D' to move and 'Space' to jump";
            }
            else
            {
                infoText.text = "Use 'A' and 'D' to move";

                if (bodiesDropdown.options[bodiesDropdown.value].text == "Arachnid 4 AxialBones (Unstable on sharp corners)")
                    infoText.text = "Unstable\n" + infoText.text;
            }
        }
        else
        {
            lineRenderer.positionCount = animator.axialBones[0].pl2d_PlayerController.pathPoints.Count;
            lineRenderer.SetPositions(animator.axialBones[0].pl2d_PlayerController.pathPoints.ToArray());

            animator.axialBones[0].pl2d_PlayerController.PlayPathFollow();

            infoText.text = "This animation follows a predefined path";
        }
    }
}
