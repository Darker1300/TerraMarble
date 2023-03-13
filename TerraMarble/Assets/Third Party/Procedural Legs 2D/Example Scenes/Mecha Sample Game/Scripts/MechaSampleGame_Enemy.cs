using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// v1.2 - "enemy" class for the Mecha scene
public class MechaSampleGame_Enemy : MechaSampleGame_Char
{
    public Transform pathsParentTransform;
    bool lateStart = true;

    public override string DescriptionText
    {
        get => "-- Spider --\n" +
                "Move: A, D\n" +
                "Jump: Space\n" +
                "Switch Character: Tab";
    }
    public override Color ColorTextImage
    {
        get => new Color(0.7455269f, 0.1781773f, 0.8584906f);
    }

    void Start()
    {
        indicatorDistance = 2;

        animator.LoadAnimation("Walk");
        mainAxialBone.playerControllerType = PL2D_AxialBone.PlayerControllerEnum.PathFollow;
        playerController.PlayPathFollow();

        foreach (Transform path in pathsParentTransform)
        {
            foreach (Transform point in path)
            {
                point.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        LoadClosestPath();
    }

    void Update()
    {
        if (lateStart)
        {
            playerController.positionTarget.SetParent(mainAxialBone.transform);
            lateStart = false;
        }
    }

    public void LoadClosestPath()
    {
        Transform closestPoint = pathsParentTransform.GetChild(0).GetChild(0);
        float minDistance = 999999;

        foreach (Transform path in pathsParentTransform)
        {
            foreach (Transform point in path)
            {
                float pointDistance = Vector2.Distance(mainAxialBone.transform.position, point.position);
                if (pointDistance < minDistance)
                {
                    minDistance = pointDistance;
                    closestPoint = point;
                }
            }
        }

        playerController.pathPoints.Clear();
        int _childCount = closestPoint.parent.childCount;
        Transform _closestPointParent = closestPoint.parent;
        for (int i = 0; i < _childCount; i++)
        {
            Transform point = _closestPointParent.GetChild(i);
            playerController.pathPoints.Add(point.position);

            if (point == closestPoint)
            {
                playerController.currentPathPoint = i;
            }
        }
    }
}
