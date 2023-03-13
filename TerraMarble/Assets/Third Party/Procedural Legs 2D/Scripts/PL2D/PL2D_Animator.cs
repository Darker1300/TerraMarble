/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Component that contains the main API.
/// It holds references to the body (Axial Bones and Legs), configuration variables and methods to manipulate the Animations (inside the folder with the GameObject's name)
/// </summary>
public class PL2D_Animator : MonoBehaviour
{
    //-- configuration variables
    public Transform rootBone;

    public enum SolverTypes { LimbSolver, ChainSolver, ManuallyAddedSolver }
    public SolverTypes solverType;
    public int chainLength;
    public Transform[] customSolverTargets;
    public System.DateTime animationsLastWriteTime;
    public string[] axialBonesSelectionPopup;
    public int axialBoneSelection;
    public string[] legsSelectionPopup;
    public int legsSelection;

    public bool clearIK;
    public bool generatePABody;

    public UnityEngine.U2D.IK.IKManager2D iKManager;

    public string animationName;
    //---

    // Animations stored in the correspondent folder
    public List<PL2D_AnimationFile> animations;

    public List<string> animationsSelectionPopup;
    public int animationSelection;

    public bool enableAanimation = true;

    // Reference to the first AxialBone (a0)
    PL2D_AxialBone mainAxialBone;

    // List of Axial Bones in the body, contains a list of the child legs
    public List<PL2D_AxialBone> axialBones;

    public bool manualSolverTargetsAdded;
    public List<Transform> allLegsTransforms;
    public PL2D_AxialBone selectedAxialBone;
    public PL2D_Leg selectedLeg;
    public int previousAnimationSelection;

    /// <summary>
    /// Clear selection variables and Axial Bones list
    /// </summary>
    public void Reset()
    {
        axialBones = new List<PL2D_AxialBone>();
        selectedAxialBone = null;
        selectedLeg = null;
    }

    public void OnValidate()
    {

        if (isActiveAndEnabled)
        {
            if (rootBone != null)
            {
                axialBones = GetAxialBones(rootBone);

                if (axialBones.Count > 0)
                {
                    if (selectedAxialBone == null)
                        selectedAxialBone = axialBones[0];
                }

                allLegsTransforms = new List<Transform>();
                foreach (Transform axialBone in GetAxialBonesTransform(rootBone, true))
                {
                    allLegsTransforms.AddRange(GetLegsTransform(axialBone));
                }

                manualSolverTargetsAdded = true;
                if (solverType == PL2D_Animator.SolverTypes.ManuallyAddedSolver)
                {
                    if (customSolverTargets == null || customSolverTargets.Length != allLegsTransforms.Count)
                        customSolverTargets = new Transform[allLegsTransforms.Count];

                    int _allLegsCount = allLegsTransforms.Count;
                    for (int i = 0; i < _allLegsCount; i++)
                    {
                        if (customSolverTargets[i] == null)
                            manualSolverTargetsAdded = false;
                    }
                }

                if (selectedAxialBone != null)
                {
                    selectedAxialBone = axialBones[axialBoneSelection];

                    selectedLeg = selectedAxialBone.legs[legsSelection];

                    if (animations == null || animationsLastWriteTime != PL2D_SaveSystem.GetAnimationsLastWriteTime(this))
                    {
                        PL2D_SaveSystem.LoadAllAnimationsToMemory(this);
                    }

                    if (selectedAxialBone.legs.Count > 0)
                    {
                        if (animations == null)
                            PL2D_SaveSystem.LoadAllAnimationsToMemory(this);

                        if (animationSelection != previousAnimationSelection)
                        {
                            if (animationSelection != animations.Count - 1)
                            {
                                if (animationSelection >= 0)
                                {
                                    LoadAnimation(animationsSelectionPopup[animationSelection]);
                                }
                            }
                        }
                        previousAnimationSelection = animationSelection;
                    }

                    if (animationSelection == animations.Count - 1)
                    {
                        if (animationName == "" || animationName == "New Animation...")
                        {
                            animationName = "Animation1";
                        }
                    }

                    if (animationSelection != animations.Count - 1)
                    {
                        if (animationSelection >= animationsSelectionPopup.Count)
                        {
                            animationSelection = animationsSelectionPopup.Count - 1;
                        }

                        if (animationSelection >= 0)
                        {
                            animationName = animationsSelectionPopup[animationSelection];
                        }
                    }

                    axialBonesSelectionPopup = new string[axialBones.Count];
                    int _axialBonesCount = axialBones.Count;
                    for (int i = 0; i < _axialBonesCount; i++)
                    {
                        axialBonesSelectionPopup[i] = axialBones[i].name;
                        if (axialBones[i].isMainAxialBone)
                            axialBonesSelectionPopup[i] += " (Main Axial Bone)";

                        if (axialBones[i].multiplierlegsDistanceOnVPosCurveVI.keys.Length <= 0)
                        {
                            axialBones[i].multiplierlegsDistanceOnVPosCurveVI.AddKey(0, 1);
                        }

                    }

                    if (solverType != PL2D_Animator.SolverTypes.ChainSolver)
                        chainLength = 3;

                    if (selectedAxialBone.autosetBodyDistanceToGround)
                    {
                        if (!Application.isPlaying)
                            selectedAxialBone.bodyDistanceToGround = Mathf.Abs((selectedAxialBone.LegsCenter).y - (selectedAxialBone.transform.position).y);

                    }

                    if (selectedAxialBone.mainBoneController.autosetRayToGroundLength)
                    {
                        if (!Application.isPlaying)
                            selectedAxialBone.RayToGroundLength = selectedAxialBone.mainBoneController.bodyDistanceToGround * 2;
                    }

                    if (selectedAxialBone.angleSolverEnabled)
                    {
                        PL2D_AngleSolver.Initialize(selectedAxialBone);
                    }
                    else
                    {
                        if (selectedAxialBone.pl2d_AngleSolver)
                            selectedAxialBone.pl2d_AngleSolver.enabled = false;
                    }

                    if (selectedAxialBone.mainBoneController.playerControllerEnabled)
                    {
                        if (selectedAxialBone.mainBoneController.pl2d_PlayerController == null)
                        {
                            selectedAxialBone.mainBoneController.pl2d_PlayerController = selectedAxialBone.mainBoneController.GetComponent<PL2D_PlayerController>();
                            PL2D_PlayerController.Initialize(selectedAxialBone.mainBoneController);
                        }

                        selectedAxialBone.mainBoneController.pl2d_PlayerController.enabled = true;
                    }
                    else
                    {
                        if (selectedAxialBone.mainBoneController.pl2d_PlayerController != null)
                            selectedAxialBone.mainBoneController.pl2d_PlayerController.enabled = false;
                    }

                    legsSelectionPopup = new string[selectedAxialBone.legs.Count];
                    int _selectedBoneLegsCount = selectedAxialBone.legs.Count;
                    for (int i = 0; i < _selectedBoneLegsCount; i++)
                    {
                        legsSelectionPopup[i] = selectedAxialBone.legs[i].name;

                        if (selectedAxialBone.legs[i].multiplierSpeedCurveVI.keys.Length <= 0)
                        {
                            selectedAxialBone.legs[i].multiplierSpeedCurveVI.AddKey(0, 1);
                        }

                        if (selectedAxialBone.legs[i].multiplierHeightCurveVI.keys.Length <= 0)
                        {
                            selectedAxialBone.legs[i].multiplierHeightCurveVI.AddKey(0, 1);
                        }

                        if (selectedAxialBone.legs[i].multiplierAngleCurveVI.keys.Length <= 0)
                        {
                            selectedAxialBone.legs[i].multiplierAngleCurveVI.AddKey(0, 1);
                        }

                        if (selectedAxialBone.legs[i].autoSetTranslationCurve)
                        {
                            if (selectedAxialBone.legs[i].stepHeightCurveSI.length > 0)
                                selectedAxialBone.legs[i].stepTranslationCurveSI = selectedAxialBone.legs[i].CreateTranslationFromHeightCurve(selectedAxialBone.legs[i].stepHeightCurveSI);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Initializes the PL2D Animator for the rigged body indicated as the rootBone. 
    /// </summary>
    /// <param name="rootBone">Parent or most parent bone of the rigged body</param>
    /// <param name="solverType">IK solver type</param>
    /// <param name="clearIK">(optional) Set true to clear the IK solvers</param>
    /// <param name="chainLength">(optional) Length of the IK chain</param>
    /// <returns>Result message</returns>
    public string Initialize(Transform rootBone, SolverTypes solverType, bool clearIK = false, int chainLength = 0)
    {
        this.rootBone = rootBone;
        this.solverType = solverType;
        this.chainLength = chainLength;

        axialBones = GetAxialBones(rootBone);

        iKManager = GetComponent<UnityEngine.U2D.IK.IKManager2D>() ? GetComponent<UnityEngine.U2D.IK.IKManager2D>() : gameObject.AddComponent<UnityEngine.U2D.IK.IKManager2D>();

        if (clearIK)
            ClearIK();

        AddAxialBones(rootBone);

        string resultMessage = PL2D_SaveSystem.LoadAllAnimationsToMemory(this);

        return "Initialization result:\n" + resultMessage;
    }

    /// <summary>
    /// Apply animation settings to the body components
    /// </summary>
    /// <param name="animationName"></param>
    /// <returns>Result message</returns>
    public string LoadAnimation(string animationName)
    {
        PL2D_SaveSystem.LoadAllAnimationsToMemory(this);

        string resultMessage = PL2D_SaveSystem.ApplyAnimationToBody(this, animationName);

        return "Apply Animation (" + animationName + ") result:\n" + resultMessage;
    }

    /// <summary>
    /// Stores the animation in the correspondent folder as JSON files
    /// </summary>
    /// <param name="animationName"></param>
    public void SaveAnimation(string animationName)
    {
        this.animationName = animationName;

        string result = PL2D_SaveSystem.SaveAnimationToFile(this, animationName);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        PL2D_SaveSystem.LoadAllAnimationsToMemory(this);

        Debug.Log(string.Format("Save {0} animation result:\n", animationName) + result);
    }

    /// <summary>
    /// Deletes the animation from the animator and its stored folder (along with the JSON files)
    /// </summary>
    /// <param name="animationName"></param>
    public void DeleteAnimation(string animationName)
    {
        this.animationName = animationName;

        string result = PL2D_SaveSystem.DeleteAnimationFile(this, animationName);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        Debug.Log(string.Format("Delete {0} animation result:\n", animationName) + result);
    }

    public void OverrideAllAxialBones(PL2D_AxialBone axialBone)
    {
        int _axialBonesCount = axialBones.Count;
        for (int i = 0; i < _axialBonesCount; i++)
        {
            PL2D_SaveSystem.ApplyAxialBoneAnimationToAxialBone(axialBone, axialBones[i]);
        }

        Debug.Log("Axial Bones overridden!");
    }

    public void OverrideAllLegsInAxialBone(PL2D_Leg leg)
    {
        int _boneLegsCount = leg.pl2d_AxialBone.legs.Count;
        for (int j = 0; j < _boneLegsCount; j++)
        {
            PL2D_SaveSystem.ApplyLegAnimationToLeg(leg, leg.pl2d_AxialBone.legs[j]);
        }
        Debug.Log("Legs in Axial Bone overridden!");
    }

    public void OverrideAllLegsInBody(PL2D_Leg leg)
    {
        int _axialBonesCount = axialBones.Count;
        for (int i = 0; i < _axialBonesCount; i++)
        {
            int _boneLegsCount = axialBones[i].legs.Count;
            for (int j = 0; j < _boneLegsCount; j++)
            {
                PL2D_SaveSystem.ApplyLegAnimationToLeg(leg, axialBones[i].legs[j]);
            }
        }
        Debug.Log("Legs overridden!");
    }

    public void ApplyDefaultSettings()
    {
        foreach (PL2D_AxialBone axialBone in axialBones)
        {
            axialBone.autosetBodyDistanceToGround = true;
            axialBone.autosetRayToGroundLength = true;
            axialBone.distanceBeforeMoveLeg = 0.1f;
            axialBone.angleBeforeMoveLeg = 0.1f;
            axialBone.bodyStabilizationTopBottomFoot = 0.8f;
            axialBone.multiplierlegsDistanceOnVPosCurveVI = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1) });
            axialBone.angleSolverEnabled = false;
            axialBone.playerControllerEnabled = false;
            axialBone.angleTrackSpeed = 10;
            
            // v1.1 - Fix: Some variables previously initialized didn't save changes
            // apply player controller default settings
            if (axialBone.isMainAxialBone)
            {
                PL2D_PlayerController.Initialize(axialBone);
                axialBone.pl2d_PlayerController.bodySpeed = 10;
                axialBone.pl2d_PlayerController.bodyAcceleration = 5;
                axialBone.pl2d_PlayerController.jumpForce = 20;
                axialBone.pl2d_PlayerController.preLegBendTime = 0.2f;
            }

            foreach (PL2D_Leg leg in axialBone.legs)
            {
                leg.stepSpeed = 2;
                leg.additionalStepLength = 0;
                leg.stepHeightCurveSI = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0) });
                leg.autoSetTranslationCurve = true;
                leg.phaseShiftFromPreviousStep = 1;
                leg.multiplierSpeedCurveVI = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1) });
                leg.additionalStepLengthCurveVI = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0) });
                leg.multiplierHeightCurveVI = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
                leg.footAngleOffset = 0;
                leg.footEndings.x = 1;
                leg.footEndings.y = 1;
                leg.footAngleCurveSI = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0) });
                leg.multiplierAngleCurveVI = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1) });
            }
        }
    }

    /// <summary>
    /// Checks if class exists
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    static bool ClassExist(string className)
    {
        Type myType = Type.GetType(className);
        return myType != null;
    }

    static public LayerMask layerMask;

    // ----- Methods to get info from boy parts names (Axial Bones or Legs) -----

    /// <summary>
    /// Get type, "a" for Axial bone or "f" for Foot
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns>type</returns>
    static public string GetTypeFromName(string fullName)
    {
        if (fullName[0] == '(')
            return fullName.Substring(1, 1);
        else
            return "0";
    }

    /// <summary>
    /// Get index of the Axial Bone or Foot
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns>index</returns>
    static public int GetIndexFromName(string fullName)
    {
        if (fullName[0] == '(')
            return int.Parse(fullName.Substring(2, 1));
        else
            return -10;
    }

    /// <summary>
    /// Get the whole identifier string, "(a[index])" for Axial Bones or "(f[index])" for Feet
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns>id string</returns>
    static public string GetShortName(string fullName)
    {
        if (fullName[0] == '(')
            return fullName.Substring(0, 4);
        else
            return "0";
    }
    // ----------

    public static void UpdateAnimationsPopup(PL2D_Animator pl2dAnimator)
    {
        string dir = PL2D_SaveSystem.ANIMATIONS_PATH + "/" + pl2dAnimator.name;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        List<string> filePaths = new List<string>();
        filePaths.AddRange(Directory.GetFiles(PL2D_SaveSystem.ANIMATIONS_PATH + "/" + pl2dAnimator.name));

        pl2dAnimator.animationsSelectionPopup = new List<string>();

        foreach (string filePath in filePaths)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            pl2dAnimator.animationsSelectionPopup.Add(fileName);
        }

        pl2dAnimator.animationsSelectionPopup.Add("New Animation...");

        pl2dAnimator.animationsLastWriteTime = PL2D_SaveSystem.GetAnimationsLastWriteTime(pl2dAnimator);
    }

    /// <summary>
    /// Clear all the solvers in the iKManager (IKManager2D component)
    /// </summary>
    private void ClearIK()
    {
        List<UnityEngine.U2D.IK.Solver2D> solvers = iKManager.solvers;
        for (int i = solvers.Count - 1; i >= 0; i--)
        {
            DeleteSolver(solvers[i]);
            iKManager.RemoveSolver(solvers[i]);
        }
    }

    private void DeleteSolver(UnityEngine.U2D.IK.Solver2D solver)
    {
#if UNITY_EDITOR
        if (solver != null)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(solver.gameObject);
            };
        }
#endif
    }

    private List<Transform> GetAxialBonesTransform(Transform bone, bool root)
    {
        List<Transform> axialBones = new List<Transform>();

        if (root)
        {
            if (PL2D_Animator.GetTypeFromName(bone.name) == "a")
            {
                axialBones.Add(bone);
            }
        }

        if (bone.childCount > 0)
        {
            foreach (Transform child in bone)
            {
                if (PL2D_Animator.GetTypeFromName(child.name) == "a")
                {
                    axialBones.Add(child);
                }

                axialBones.AddRange(GetAxialBonesTransform(child, false));
            }
        }

        return axialBones;
    }

    private List<PL2D_AxialBone> GetAxialBones(Transform bone)
    {
        List<PL2D_AxialBone> axialBones = new List<PL2D_AxialBone>();
        PL2D_AxialBone foundAxialBone = null;

        foreach (Transform axialBoneTransform in GetAxialBonesTransform(bone, true))
        {
            foundAxialBone = axialBoneTransform.gameObject.GetComponent<PL2D_AxialBone>();
            if (foundAxialBone != null)
            {
                axialBones.Add(foundAxialBone);
                foundAxialBone.pL2dAnimator = this;
                foundAxialBone.mainBoneController = axialBones[0];
            }
        }

        return axialBones;
    }

    private void AddAxialBones(Transform bone)
    {
        int legCount = 0;
        foreach (Transform axialBone in GetAxialBonesTransform(bone, true))
        {
            PL2D_AxialBone pl2d_AxialBone = axialBone.gameObject.GetComponent<PL2D_AxialBone>() ? axialBone.gameObject.GetComponent<PL2D_AxialBone>() : axialBone.gameObject.AddComponent<PL2D_AxialBone>();

            if (PL2D_Animator.GetIndexFromName(axialBone.name) == 0)
            {
                pl2d_AxialBone.isMainAxialBone = true;
                mainAxialBone = pl2d_AxialBone;
            }
            else
            {
                pl2d_AxialBone.isMainAxialBone = false;
                pl2d_AxialBone.RayToGroundLength = mainAxialBone.RayToGroundLength;

                pl2d_AxialBone.mainBoneController = mainAxialBone;
            }

            pl2d_AxialBone.distanceBeforeMoveLeg = 0.2f;
            pl2d_AxialBone.legs = new List<PL2D_Leg>();
            pl2d_AxialBone.bodyStabilizationTopBottomFoot = 0.3f;

            pl2d_AxialBone.multiplierlegsDistanceOnVPosCurveVI = new AnimationCurve();

            pl2d_AxialBone.axialBoneIndex = PL2D_Animator.GetIndexFromName(pl2d_AxialBone.name);

            AddLegs(axialBone.transform, pl2d_AxialBone);

            if (solverType == SolverTypes.ManuallyAddedSolver)
            {
                foreach (PL2D_Leg leg in pl2d_AxialBone.legs)
                {
                    leg.limbTarget = customSolverTargets[legCount];
                    legCount++;
                }
            }

        }

    }

    private List<Transform> GetLegsTransform(Transform parentAxialBone)
    {
        List<Transform> legs = new List<Transform>();

        if (parentAxialBone.childCount > 0)
        {
            foreach (Transform child in parentAxialBone)
            {
                if (PL2D_Animator.GetTypeFromName(child.name) == "f")
                {

                    if (!legs.Contains(child))
                        legs.Add(child);
                }
                if (PL2D_Animator.GetTypeFromName(child.name) == "a")
                {
                    break;
                }
                else
                {
                    legs.AddRange(GetLegsTransform(child));
                }
            }
        }

        return legs;
    }

    private List<PL2D_Leg> GetLegs(Transform parentAxialBone)
    {
        List<PL2D_Leg> legs = new List<PL2D_Leg>();

        if (parentAxialBone.childCount > 0)
        {
            foreach (Transform legTransform in GetLegsTransform(parentAxialBone))
            {
                if (legTransform.gameObject.GetComponent<PL2D_Leg>())
                    legs.Add(legTransform.gameObject.GetComponent<PL2D_Leg>());
            }
        }

        return legs;
    }

    private void AddLegs(Transform parent, PL2D_AxialBone pl2d_AxialBone)
    {
        List<Transform> legsTransform = GetLegsTransform(pl2d_AxialBone.transform);

        foreach (Transform leg in legsTransform)
        {
            PL2D_Leg pl2d_Leg = leg.gameObject.GetComponent<PL2D_Leg>() ? leg.gameObject.GetComponent<PL2D_Leg>() : leg.gameObject.AddComponent<PL2D_Leg>();

            pl2d_Leg.pl2d_AxialBone = pl2d_AxialBone;

            pl2d_Leg.multiplierAngleCurveVI = new AnimationCurve();
            pl2d_Leg.multiplierHeightCurveVI = new AnimationCurve();
            pl2d_Leg.multiplierSpeedCurveVI = new AnimationCurve();
            pl2d_Leg.additionalStepLengthCurveVI = new AnimationCurve();
            pl2d_Leg.footAngleCurveSI = new AnimationCurve();
            pl2d_Leg.stepHeightCurveSI = new AnimationCurve();
            pl2d_Leg.stepTranslationCurveSI = new AnimationCurve();

            if (solverType != SolverTypes.ManuallyAddedSolver)
            {
                AddSolverToLeg(pl2d_Leg);
            }

            pl2d_AxialBone.legs.Add(pl2d_Leg);

            GameObject newLimbCenter = pl2d_Leg.limbCenter == null ? new GameObject("LimbCenter " + leg.name) : pl2d_Leg.limbCenter.gameObject;
            pl2d_Leg.limbCenter = newLimbCenter.transform;
            newLimbCenter.transform.SetParent(pl2d_AxialBone.transform);
            newLimbCenter.transform.position = new Vector3(pl2d_Leg.transform.position.x, pl2d_AxialBone.transform.position.y);

            pl2d_Leg.legIndex = PL2D_Animator.GetIndexFromName(pl2d_Leg.name);
        }
    }

    private void AddSolverToLeg(PL2D_Leg pl2d_Leg)
    {
        GameObject solverGO = new GameObject("solver " + PL2D_Animator.GetShortName(pl2d_Leg.name) + " - " + PL2D_Animator.GetShortName(pl2d_Leg.pl2d_AxialBone.name));
        solverGO.transform.position = pl2d_Leg.transform.position;

        solverGO.transform.SetParent(transform);

        UnityEngine.U2D.IK.Solver2D solver2D = null;
        if (solverType == SolverTypes.LimbSolver)
        {
            solver2D = solverGO.AddComponent<UnityEngine.U2D.IK.LimbSolver2D>();
        }
        else if (solverType == SolverTypes.ChainSolver)
        {
            solver2D = solverGO.AddComponent<UnityEngine.U2D.IK.CCDSolver2D>();
            solver2D.GetChain(0).transformCount = chainLength;
        }

        solver2D.GetChain(0).effector = pl2d_Leg.transform;

        pl2d_Leg.limbTarget = solverGO.transform;
        pl2d_Leg.solver2D = solver2D;

        if (solverType != SolverTypes.ManuallyAddedSolver)
            iKManager.AddSolver(solver2D);
    }

    void Start()
    {
        PL2D_SaveSystem.LoadAllAnimationsToMemory(this);

        OnValidate();
    }
}