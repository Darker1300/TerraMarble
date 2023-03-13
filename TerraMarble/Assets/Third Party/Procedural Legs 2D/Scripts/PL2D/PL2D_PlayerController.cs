/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component enables movement of the body with keys or following path points
/// </summary>
public class PL2D_PlayerController : MonoBehaviour
{
    public float bodySpeed;
    public float bodyAcceleration;
    public bool enableWallWalk;
    public bool enableJump;
    public float jumpForce;
    public float horizontalVelocityMultiplierForce;
    public float preLegBendTime;
    public bool enableFlipToMouse;
    public bool invertFlipToMouse;
    public Transform positionTarget;
    public PL2D_AxialBone pl2d_AxialBone;

    Vector3 startScale;
    Vector3 bodyPosition;
    float height = 0.5f;
    float rayDistance = 5;
    int canFlip = 0; // 0 = do nothing, 1 = flip right, 2 = flip left;

    public bool isFlipped;
    public Rigidbody2D rig2d;

    /// <summary>
    /// Adds and initializes a PL2D_PlayerController component in the Axial Bone
    /// </summary>
    /// <param name="axialBone"></param>
    public static void Initialize(PL2D_AxialBone axialBone)
    {
        axialBone.pl2d_PlayerController = axialBone.GetComponent<PL2D_PlayerController>() ? axialBone.GetComponent<PL2D_PlayerController>() : axialBone.gameObject.AddComponent<PL2D_PlayerController>();

        axialBone.pl2d_PlayerController.rig2d = axialBone.pl2d_PlayerController.rig2d == null ? axialBone.gameObject.AddComponent<Rigidbody2D>() : axialBone.GetComponent<Rigidbody2D>();

        axialBone.pl2d_PlayerController.positionTarget = axialBone.pl2d_PlayerController.positionTarget ? axialBone.pl2d_PlayerController.positionTarget : new GameObject("PlayerControllerPositionTarget").transform;
        axialBone.pl2d_PlayerController.positionTarget.SetParent(axialBone.transform);
        axialBone.pl2d_PlayerController.positionTarget.position = new Vector3(axialBone.transform.position.x, axialBone.LegsCenter.y);

        axialBone.pl2d_PlayerController.pl2d_AxialBone = axialBone;
    }

    void OnValidate()
    {
        startScale = transform.localScale;
    }

    void OnEnable()
    {
        startScale = transform.localScale;
    }

    void Start()
    {
        if (rig2d)
            rig2d.bodyType = RigidbodyType2D.Dynamic;

        if (positionTarget)
            positionTarget.SetParent(pl2d_AxialBone.pL2dAnimator.transform);
    }

    void Update()
    {
        // v1.2 - fix: setting playerControllerEnable didn't work
        if (pl2d_AxialBone.PlayerControllerEnabled)
        {
            // jump key check (for some reason, Space is KeuCode.O on Ubuntu)
            JumpCheck(KeyCode.Space);
            JumpCheck(KeyCode.O);
        }
    }

    public bool pressedJump = false;

    void FixedUpdate()
    {
        // v1.2 - fix: setting playerControllerEnable didn't work
        if (pl2d_AxialBone.PlayerControllerEnabled)
        {
            if (pl2d_AxialBone.playerControllerType == PL2D_AxialBone.PlayerControllerEnum.PathFollow)
            {
                if (playPathFollow)
                {
                    WalkPath();
                }
            }
            else if (pl2d_AxialBone.playerControllerType == PL2D_AxialBone.PlayerControllerEnum.KeyboardInput)
            {
                if (pl2d_AxialBone.isGrounded && !pl2d_AxialBone.isJumping)
                {
                    // calc body position
                    {
                        bodyPosition = transform.position;

                        if (pl2d_AxialBone.isMainAxialBone)
                        {
                            float legsCenterDistToGround = 0;
                            // limit the body rise 
                            if (pl2d_AxialBone.RelativePoint(pl2d_AxialBone.axialBoneHit.point).y - pl2d_AxialBone.RelativePoint(pl2d_AxialBone.LegsCenter).y < pl2d_AxialBone.bodyDistanceToGround)
                                legsCenterDistToGround = Mathf.Abs(pl2d_AxialBone.RelativePoint(pl2d_AxialBone.LegsCenter).y - pl2d_AxialBone.RelativePoint(pl2d_AxialBone.axialBoneHit.point).y);

                            float targetToLegsCenterDist = pl2d_AxialBone.RelativePoint(positionTarget.position).y < pl2d_AxialBone.RelativePoint(pl2d_AxialBone.LegsCenter).y ? 0 : Mathf.Abs(pl2d_AxialBone.RelativePoint(positionTarget.position).y - pl2d_AxialBone.RelativePoint(pl2d_AxialBone.LegsCenter).y);

                            Vector3 newPos;
                            // v1.1 - Fix: Vertical body positioning multiplier, improved "Wall Walk" disabled movement. Recommended to use "wall walk" only for soft angled and round corners grounds.
                            if (enableWallWalk)
                                newPos = positionTarget.position + (transform.up * ((pl2d_AxialBone.bodyDistanceToGround * pl2d_AxialBone.multiplierlegsDistanceOnVPosCurveVI.Evaluate(Mathf.Abs(pl2d_AxialBone.bodyVelocity / bodySpeed))) - targetToLegsCenterDist + (legsCenterDistToGround - legsCenterDistToGround / 2)));
                            //newPos = positionTarget.position + (transform.up * (pl2d_AxialBone.bodyDistanceToGround - targetToLegsCenterDist + (legsCenterDistToGround - legsCenterDistToGround / 2))
                            //* pl2d_AxialBone.multiplierlegsDistanceOnVPosCurveVI.Evaluate(Mathf.Abs(pl2d_AxialBone.bodyVelocity / bodySpeed)));
                            else
                                newPos = new Vector3(positionTarget.position.x, (pl2d_AxialBone.LegsCenter.y + (pl2d_AxialBone.bodyDistanceToGround * pl2d_AxialBone.multiplierlegsDistanceOnVPosCurveVI.Evaluate(Mathf.Abs(pl2d_AxialBone.bodyVelocity / bodySpeed)))), 0);

                            Vector3 pos = bodyPosition;
                            // v1.2 - adjusted body y lerp velocity
                            pos.y = Mathf.Lerp(pos.y, newPos.y, Time.deltaTime * -Physics2D.gravity.y / 2);
                            pos.x = Mathf.Lerp(pos.x, newPos.x, Time.deltaTime * bodyAcceleration);
                            bodyPosition = pos;
                        }

                        transform.position = bodyPosition;
                    }
                }

                if (enableJump)
                {
                    if (pressedJump && !pl2d_AxialBone.isJumping)
                    {
                        if (pl2d_AxialBone.isGrounded && pl2d_AxialBone.axialBoneIndex == 0)
                        {
                            StopAllCoroutines();
                            pl2d_AxialBone.isMoving = false;
                            foreach (PL2D_Leg leg in pl2d_AxialBone.legs)
                            {
                                leg.isMoving = false;
                            }
                            StartCoroutine(Jump(preLegBendTime));
                        }
                        pressedJump = false;
                    }
                }

                Move(KeyCode.A, KeyCode.D);

            }

            if (enableFlipToMouse)
            {
                // flip vertical based on mouse position
                FlipBodyOnMousePos();
            }
        }
    }

    // v1.1 - PL2D_PlayerController.Move refactored to facilitate reuse and customization, added keycode parameters 
    /// <summary>
    /// Checks jump key and pass true to start jump coroutine
    /// </summary>
    /// <param name="jumpKey"></param>
    public void JumpCheck(KeyCode jumpKey)
    {
        // v1.2 - fix: avoid jump while arlready jumping 
        if (!pl2d_AxialBone.isJumping && Input.GetKeyDown(jumpKey) && pl2d_AxialBone.playerControllerType == PL2D_AxialBone.PlayerControllerEnum.KeyboardInput)
        {
            pressedJump = true;
        }
    }

    // v1.1 - PL2D_PlayerController.Move refactored to facilitate reuse and customization, added keycode parameters 
    /// <summary>
    /// Translates and adjusts positionTarget to the passed direction and resolves the body position 
    /// </summary>
    /// <param name="dir"></param>
    public void Move(KeyCode leftKey, KeyCode rightKey)
    {
        if (pl2d_AxialBone.isGrounded && !pl2d_AxialBone.isJumping)
        {
            Vector3 dir = new Vector3(0,0,0);
            if (Input.GetKey(leftKey))
            {
                dir = -new Vector3(1,0,0);
            }
            if (Input.GetKey(rightKey))
            {
                dir = new Vector3(1,0,0);
            }

            RaycastHit2D hit;
            var up = positionTarget.up;

            hit = Physics2D.CircleCast(positionTarget.position, height, -up, rayDistance, PL2D_Animator.layerMask);
            Debug.DrawLine(positionTarget.position, hit.point);

            RaycastHit2D hitNormal = Physics2D.Raycast(transform.position, -hit.normal, 200, PL2D_Animator.layerMask);

            if (enableWallWalk)
            {
                if (hit.transform)
                {
                    positionTarget.position = hit.point + hit.normal * height;
                    positionTarget.Translate(dir * bodySpeed * Time.deltaTime);

                    float angle = Mathf.LerpAngle(positionTarget.eulerAngles.z,
                                                    90 + PL2D_AngleSolver.Atan2Angle(hitNormal.point, transform.position),
                                                    Time.deltaTime * pl2d_AxialBone.mainBoneController.angleTrackSpeed);
                    positionTarget.eulerAngles = new Vector3(0, 0, angle);

                }
                else
                {
                    positionTarget.up = pl2d_AxialBone.transform.up;
                    // v1.1 - Fix: body occasionally getting stuck in the air if hit.transform == null
                    pl2d_AxialBone.SetIsGrounded(false);
                }
            }
            else
            {
                positionTarget.Translate(dir * bodySpeed * Time.deltaTime);
                if (Mathf.Abs(positionTarget.position.x - transform.position.x) > bodyAcceleration)
                {
                    positionTarget.Translate(-dir * bodySpeed * Time.deltaTime);
                }
            }

        }
    }

    // v1.2 - jump control refactored and improved:
    // - removed duplicade LerpLegToPosition behavior on grounded
    // - adjusted while conditions
    // - jumpForce now corresponds to the force aplyied by RigdBody.AddForce()
    // - added jump variable "horizontalVelocityMultiplierForce"
    // v1.1 - Improved Jump, added pre jump leg bend time setting
    // v1.1.1 - Fix: Glitch on jumping on Raycast angle solver type. Removed setting isJumping from the middle of the coroutine
    /// <summary>
    /// Apply translations and forces to make the body jump while setting up the Jump state
    /// </summary>
    /// <param name="preKneeBendTime">Duration of the pre jump leg bending</param>
    /// <returns></returns>
    IEnumerator Jump(float preLegBendTime)
    {
        float count = 0;
        float initialAbsVelocity = Mathf.Abs(pl2d_AxialBone.bodyVelocity);
        Vector3 initDirection = pl2d_AxialBone.MoveDirection.normalized;
        Vector3 initUp = pl2d_AxialBone.transform.up;

        pl2d_AxialBone.isJumping = true;

        while (count <= preLegBendTime)
        {
            count += Time.deltaTime;
            if (count > 1)
                count = 1;

            pl2d_AxialBone.transform.position -= (pl2d_AxialBone.transform.up * 0.1f);

            rig2d.velocity = Vector2.zero;

            yield return null;
        }

        rig2d.AddForce((initUp * jumpForce) + (initDirection * initialAbsVelocity * horizontalVelocityMultiplierForce));

        count = 0;
        while (count <= preLegBendTime)
        {
            count += Time.deltaTime;
            pl2d_AxialBone.rayToGroundLength = 0.1f;

            yield return null;
        }

        yield return null;
    }

    // v1.1 - FlipBodyOnMousePos refactored and added FlipBody to facilitate reuse
    /// <summary>
    /// Make the body turn to the mouse
    /// </summary>
    int lastFlipDirection;
    public void FlipBodyOnMousePos()
    {
        var mouse_pos = Input.mousePosition;
        mouse_pos.z = 10;
        var mousePosition = Camera.main.ScreenToWorldPoint(mouse_pos);
        var transform_pos = pl2d_AxialBone.transform.position;

        if (!invertFlipToMouse)
        {
            if (mousePosition.x > transform_pos.x)
            {
                canFlip = 1;
            }
            else
            {
                canFlip = 2;
            }
        }
        else
        {
            if (mousePosition.x < transform_pos.x)
            {
                canFlip = 1;
            }
            else
            {
                canFlip = 2;
            }
        }

        FlipBody(canFlip);
    }

    // v1.1 - FlipBodyOnMousePos refactored and added FlipBody to facilitate reuse
    /// <summary>
    /// Flip body
    /// </summary>
    /// <param name="flipDirection">values 1 or 2</param>
    public void FlipBody(int flipDirection)
    {
        if (flipDirection != lastFlipDirection)
        {
            if (flipDirection == 1)
            {
                pl2d_AxialBone.transform.localScale = startScale;
                isFlipped = false;
            }
            else if (flipDirection == 2)
            {
                pl2d_AxialBone.transform.localScale = new Vector3(-startScale.x, startScale.y, startScale.z);
                isFlipped = true;
            }

            RaycastHit2D flipHit;
            foreach (PL2D_AxialBone axialBone in pl2d_AxialBone.pL2dAnimator.axialBones)
            {
                foreach (PL2D_Leg leg in axialBone.legs)
                {
                    flipHit = Physics2D.Raycast(leg.transform.position, -transform.up, 100, PL2D_Animator.layerMask);

                    StartCoroutine(leg.LerpLegToPosition(flipHit.point));
                }
            }

            lastFlipDirection = flipDirection;
        }
    }

    public List<Vector3> pathPoints;
    // v1.2 - currentPathPoint made public for acess
    public int currentPathPoint = 0;
    bool pathFollowForward = true;
    bool playPathFollow;

    /// <summary>
    /// Set active path follow state 
    /// </summary>
    public void PlayPathFollow()
    {
        if (pathPoints.Count > 0)
        {
            playPathFollow = true;
        }
    }

    /// <summary>
    /// Set inactive the path follow state 
    /// </summary>
    public void PausePathFollow()
    {
        playPathFollow = false;
    }

    static GameObject GetChildByNameInParent(string childName, Transform parent)
    {
        GameObject foundChild = null;
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                foundChild = child.gameObject;
                break;
            }
        }
        return foundChild;
    }

    /// <summary>
    /// Adds a point to the path points list
    /// </summary>
    /// <param name="position"></param>
    /// <param name="index"></param>
    public void AddPathPoint(Vector3 position, int index)
    {
        pathPoints.Insert(index, position);
    }

    /// <summary>
    /// Removes point to the path points list
    /// </summary>
    /// <param name="index"></param>
    public void RemovePathPoint(int index)
    {
        if (pathPoints[index] != null)
        {
            pathPoints.RemoveAt(index);
        }
    }

    /// <summary>
    /// Sequentially translates the body towards each path point 
    /// </summary>
    void WalkPath()
    {
        transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPathPoint], bodySpeed * Time.deltaTime);

        if (transform.position == pathPoints[currentPathPoint])
        {
            if (currentPathPoint >= pathPoints.Count - 1)
            {
                pathFollowForward = false;
            }
            if (currentPathPoint <= 0)
            {
                pathFollowForward = true;
            }

            if (pathPoints.Count > 1)
            {
                if (pathFollowForward)
                {
                    currentPathPoint++;
                }
                else
                {
                    currentPathPoint--;
                }
            }
        }
    }
}
