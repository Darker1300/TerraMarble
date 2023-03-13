/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

/// <summary>
/// Static class that holds the API for saving to files and loading animations from the files to the body
/// The files are stored in the folder "/Procedural Legs 2D/Resources/PL2D Animations/[PL2D Animator Game Object' name]/[animation name]"
/// </summary>
public static class PL2D_SaveSystem
{
    // Main save path
    public static string ANIMATIONS_PATH = Application.dataPath + "/Procedural Legs 2D/Resources/PL2D Animations";

    /// <summary>
    /// Converts the animation values into JSON and save them in the correspondent folder
    /// </summary>
    /// <param name="pl2dAnimator">Animator that holds the reference to the body and its PL2D components</param>
    /// <param name="animationName">Identifier for the animation</param>
    /// <returns>Result message</returns>
    public static string SaveAnimationToFile(PL2D_Animator pl2dAnimator, string animationName)
    {
        string specificAnimationPath = ANIMATIONS_PATH + "/" + pl2dAnimator.name + "/" + animationName;

        if (!Directory.Exists(ANIMATIONS_PATH + "/" + pl2dAnimator.name))
            Directory.CreateDirectory(ANIMATIONS_PATH + "/" + pl2dAnimator.name);

        if (!Directory.Exists(specificAnimationPath))
            Directory.CreateDirectory(specificAnimationPath);

        int axialBonesCount = 0;
        int legsCount = 0;
        foreach (PL2D_AxialBone axialBone in pl2dAnimator.axialBones)
        {
            if (axialBone.isMainAxialBone)
            {
                string playerControllerJsonString = JsonUtility.ToJson(axialBone.pl2d_PlayerController);
                File.WriteAllText(specificAnimationPath + "/PlayerController.json", playerControllerJsonString);
            }

            string axialBoneJsonString = JsonUtility.ToJson(axialBone);
            File.WriteAllText(specificAnimationPath + "/" + PL2D_Animator.GetShortName(axialBone.name) + ".json", axialBoneJsonString);
            axialBonesCount++;

            foreach (PL2D_Leg leg in axialBone.legs)
            {
                string legJsonString = JsonUtility.ToJson(leg);
                File.WriteAllText(specificAnimationPath + "/" + PL2D_Animator.GetShortName(leg.name) + "_" + PL2D_Animator.GetShortName(axialBone.name) + ".json", legJsonString);
                legsCount++;
            }
        }

        return string.Format("Animation saved with ({0} Axial Bones) and ({1} Legs) at ({2})", axialBonesCount, legsCount, specificAnimationPath);
    }

    /// <summary>
    /// Delete the folder that have the JSON animation files
    /// </summary>
    /// <param name="pl2dAnimator">Animator that holds the reference to the body and its PL2D components</param>
    /// <param name="animationName">Identifier</param>
    /// <returns>Result message</returns>
    public static string DeleteAnimationFile(PL2D_Animator pl2dAnimator, string animationName)
    {
        string result = DeleteAnimationFromBodyMemory(pl2dAnimator, animationName);

        string specificAnimationPath = ANIMATIONS_PATH + "/" + pl2dAnimator.name + "/" + animationName;

        if (Directory.Exists(specificAnimationPath))
        {
            Directory.Delete(specificAnimationPath, true);
            return result += "\nAnimation file removed";
        }
        else
        {
            return result += "\nAnimation file does not exist";
        }
    }

    /// <summary>
    /// Delete all the folders in the correspondent PL2D Animator folder
    /// </summary>
    /// <param name="pl2dAnimator"></param>
    public static void DeleteAllAnimationFiles(PL2D_Animator pl2dAnimator)
    {
        string animatorPath = ANIMATIONS_PATH + "/" + pl2dAnimator.name;

        if (Directory.Exists(animatorPath))
        {
            List<string> filePaths = new List<string>();
            filePaths.AddRange(Directory.GetFiles(animatorPath));
            foreach (string filePath in filePaths)
            {
                if (Directory.Exists(filePath))
                {
                    string dirName = Path.GetFileNameWithoutExtension(filePath);
                    DeleteAnimationFromBodyMemory(pl2dAnimator, dirName);

                    Directory.Delete(filePath);
                }
            }
        }
    }

    /// <summary>
    /// Clears the PL2D Animator' animations list
    /// </summary>
    /// <param name="pl2dAnimator"></param>
    public static void DeleteAllAnimationsFromBodyMemory(PL2D_Animator pl2dAnimator)
    {
        pl2dAnimator.animations.Clear();
    }

    /// <summary>
    /// Removes the correspondent animation rom the PL2D Animator' animations list
    /// </summary>
    /// <param name="pl2dAnimator"></param>
    /// <param name="animationName"></param>
    /// <returns>Result message</returns>
    public static string DeleteAnimationFromBodyMemory(PL2D_Animator pl2dAnimator, string animationName)
    {
        PL2D_AnimationFile animationFile = pl2dAnimator.animations.Find(a => a.name == animationName);
        if (animationFile != null)
        {
            pl2dAnimator.animations.Remove(animationFile);
            return "Animation removed from memory";
        }
        else
        {
            return "Animation does not exist in memory";
        }
    }

    /// <summary>
    /// Initialize and apply the animation settings to the PL2D_PlayerController component
    /// </summary>
    /// <param name="playerControllerAnimation"></param>
    /// <param name="axialBone"></param>
    public static void ApplyPlayerControllerAnimationToAxialBone(PL2D_PlayerController playerControllerAnimation, PL2D_AxialBone axialBone)
    {
        if (axialBone.GetComponent<PL2D_PlayerController>() == null)
        {
            PL2D_PlayerController.Initialize(axialBone);
        }

        axialBone.pl2d_PlayerController.bodySpeed = playerControllerAnimation.bodySpeed;
        axialBone.pl2d_PlayerController.bodyAcceleration = playerControllerAnimation.bodyAcceleration;
        axialBone.pl2d_PlayerController.enableWallWalk = playerControllerAnimation.enableWallWalk;
        axialBone.pl2d_PlayerController.enableJump = playerControllerAnimation.enableJump;
        axialBone.pl2d_PlayerController.jumpForce = playerControllerAnimation.jumpForce;
        axialBone.pl2d_PlayerController.horizontalVelocityMultiplierForce = playerControllerAnimation.horizontalVelocityMultiplierForce;
        axialBone.pl2d_PlayerController.preLegBendTime = playerControllerAnimation.preLegBendTime;
        axialBone.pl2d_PlayerController.enableFlipToMouse = playerControllerAnimation.enableFlipToMouse;
        axialBone.pl2d_PlayerController.invertFlipToMouse = playerControllerAnimation.invertFlipToMouse;
        axialBone.pl2d_PlayerController.pathPoints = playerControllerAnimation.pathPoints;
    }

    /// <summary>
    /// Initialize and apply the animation settings to the PL2D_AxialBone component
    /// </summary>
    /// <param name="axialBoneAnimation"></param>
    /// <param name="axialBone"></param>
    public static void ApplyAxialBoneAnimationToAxialBone(PL2D_AxialBone axialBoneAnimation, PL2D_AxialBone axialBone)
    {
        if (axialBone.GetComponent<PL2D_AngleSolver>() == null)
        {
            PL2D_AngleSolver.Initialize(axialBone);
            axialBone.pl2d_AngleSolver.enabled = false;
        }

        axialBone.autosetBodyDistanceToGround = axialBoneAnimation.autosetBodyDistanceToGround;
        axialBone.bodyDistanceToGround = axialBoneAnimation.bodyDistanceToGround;
        axialBone.autosetRayToGroundLength = axialBoneAnimation.autosetRayToGroundLength;
        axialBone.RayToGroundLength = axialBoneAnimation.RayToGroundLength;
        axialBone.distanceBeforeMoveLeg = axialBoneAnimation.distanceBeforeMoveLeg;
        axialBone.angleBeforeMoveLeg = axialBoneAnimation.angleBeforeMoveLeg;
        axialBone.bodyStabilizationTopBottomFoot = axialBoneAnimation.bodyStabilizationTopBottomFoot;
        axialBone.multiplierlegsDistanceOnVPosCurveVI.keys = axialBoneAnimation.multiplierlegsDistanceOnVPosCurveVI.keys;
        axialBone.angleSolverType = axialBoneAnimation.angleSolverType;
        axialBone.angleTrackSpeed = axialBoneAnimation.angleTrackSpeed;
        axialBone.angleSolverEnabled = axialBoneAnimation.angleSolverEnabled;
        axialBone.playerControllerEnabled = axialBoneAnimation.playerControllerEnabled;
        axialBone.playerControllerType = axialBoneAnimation.playerControllerType;
    }

    /// <summary>
    /// Initialize and apply the animation settings to the PL2D_Leg component
    /// </summary>
    /// <param name="legAnimation"></param>
    /// <param name="leg"></param>
    public static void ApplyLegAnimationToLeg(PL2D_Leg legAnimation, PL2D_Leg leg)
    {
        leg.stepSpeed = legAnimation.stepSpeed;
        leg.additionalStepLength = legAnimation.additionalStepLength;
        leg.stepHeightCurveSI.keys = legAnimation.stepHeightCurveSI.keys;
        leg.autoSetTranslationCurve = legAnimation.autoSetTranslationCurve;
        leg.stepTranslationCurveSI.keys = legAnimation.stepTranslationCurveSI.keys;
        leg.phaseShiftFromPreviousStep = legAnimation.phaseShiftFromPreviousStep;
        leg.multiplierSpeedCurveVI.keys = legAnimation.multiplierSpeedCurveVI.keys;
        leg.additionalStepLengthCurveVI.keys = legAnimation.additionalStepLengthCurveVI.keys;
        leg.multiplierHeightCurveVI.keys = legAnimation.multiplierHeightCurveVI.keys;
        leg.footEndings = legAnimation.footEndings;
        leg.footAngleOffset = legAnimation.footAngleOffset;
        leg.footAngleCurveSI.keys = legAnimation.footAngleCurveSI.keys;
        leg.multiplierAngleCurveVI.keys = legAnimation.multiplierAngleCurveVI.keys;
    }

    /// <summary>
    /// Initializer needed components and apply animation settings tho the body
    /// </summary>
    /// <param name="pl2dAnimator"></param>
    /// <param name="animationName"></param>
    /// <returns>Result message</returns>
    public static string ApplyAnimationToBody(PL2D_Animator pl2dAnimator, string animationName)
    {
        PL2D_AnimationFile animationFile = pl2dAnimator.animations.Find(a => a.name == animationName);

        string info = "Success";

        if (animationFile != null)
        {
            List<PL2D_AxialBone> bodyAxialBones = pl2dAnimator.axialBones;
            foreach (PL2D_AxialBone bodyAxialBone in bodyAxialBones)
            {
                PL2D_AxialBoneFile axialBoneFile = null;
                axialBoneFile = animationFile.axialBones.Find(a => a.name == PL2D_Animator.GetShortName(bodyAxialBone.name));

                if (axialBoneFile != null)
                {
                    // v1.2 - fix: animation name not updating
                    pl2dAnimator.animationName = animationName;

                    // v1.2 - fix: warnings on creating body parts from json
                    // load Axial Bone file
                    axialBoneFile.FromJsonToAxialBone(bodyAxialBone);

                    if (bodyAxialBone.isMainAxialBone)
                    {
                        if (animationFile.playerController != null)
                        {
                            // v1.2 - fix: warnings on creating body parts from json
                            PL2D_PlayerControllerFile playerControllerFile = animationFile.playerController;
                            playerControllerFile.FromJsonToPlayerController(bodyAxialBone);
                        }
                    }

                    foreach (PL2D_Leg bodyLeg in bodyAxialBone.legs)
                    {
                        PL2D_LegFile legFile = null;
                        legFile = axialBoneFile.legs.Find(l => l.name == PL2D_Animator.GetShortName(bodyLeg.name));
                        if (legFile != null)
                        {
                            legFile.FromJsonToLeg(bodyLeg);

                            // v1.2 - fix: warnings on creating body parts from json
                            // load leg file
                            legFile.FromJsonToLeg(bodyLeg);
                        }
                        else
                        {
                            info = "Leg File not found - " + PL2D_Animator.GetShortName(bodyLeg.name);
                        }

                    }
                }
                else
                {
                    info = "Axial Bone File not found - " + PL2D_Animator.GetShortName(bodyAxialBone.name);
                }

                PL2D_AngleSolver.Initialize(bodyAxialBone);
            }
        }
        else
        {
            info = "no animations in body memory";
        }

        return info;
    }

    /// <summary>
    /// Populate the PL2D Animator's animations list with all the stored animations
    /// </summary>
    /// <param name="pl2dAnimator"></param>
    /// <returns>Result message</returns>
    public static string LoadAllAnimationsToMemory(PL2D_Animator pl2dAnimator)
    {
#if UNITY_EDITOR
        PL2D_Animator.UpdateAnimationsPopup(pl2dAnimator);
#endif

        if (pl2dAnimator.animations != null)
        {
            pl2dAnimator.animations.Clear();
        }
        else
        {
            pl2dAnimator.animations = new List<PL2D_AnimationFile>();
        }

        string resultMessage = "";

        foreach (string s in pl2dAnimator.animationsSelectionPopup)
        {
            string info = LoadAnimationToBodyMemory(pl2dAnimator, s);
            resultMessage += info + "\n";
        }


        return resultMessage;

    }

    /// <summary>
    /// Adds the correspondent animation to the PL2D Animator's animations list 
    /// </summary>
    /// <param name="pl2dAnimator"></param>
    /// <param name="animationName"></param>
    /// <returns>Result message</returns>
    public static string LoadAnimationToBodyMemory(PL2D_Animator pl2dAnimator, string animationName)
    {
        string resultMessage = "";

        PL2D_AnimationFile animationFile = new PL2D_AnimationFile(animationName, GetAxialBoneFiles(animationName, pl2dAnimator), GetPlayerControllerFile(animationName, pl2dAnimator));

        if (animationFile != null)
        {
            if (pl2dAnimator.animations.Find(p => p.name == animationName) == null)
            {
                pl2dAnimator.animations.Add(animationFile);

                resultMessage = "Success: Animation loaded - " + animationName;
            }
            else
            {
                resultMessage = "Warning: Animation already Loaded - " + animationName;
            }
        }
        else
        {
            resultMessage = "Error: Animation doesn't exist - " + animationName;
        }

        return resultMessage;
    }

    public static System.DateTime GetAnimationsLastWriteTime(PL2D_Animator pl2dAnimator)
    {
        string animatorPath = ANIMATIONS_PATH + "/" + pl2dAnimator.name;

        return Directory.GetLastWriteTime(animatorPath);
    }

    /// <summary>
    /// Converts the player controller JSON file from the saved animation to PL2D_PlayerControllerFile
    /// </summary>
    /// <param name="animationName"></param>
    /// <param name="pl2dAnimator"></param>
    /// <returns></returns>
    private static PL2D_PlayerControllerFile GetPlayerControllerFile(string animationName, PL2D_Animator pl2dAnimator)
    {
        PL2D_PlayerControllerFile playerControllerFile = null;

        foreach (TextAsset filePath in Resources.LoadAll("PL2D Animations/" + pl2dAnimator.name + "/" + animationName, typeof(TextAsset)))
        {
            string fileName = filePath.name;
            if (fileName == "PlayerController")
            {
                playerControllerFile = new PL2D_PlayerControllerFile(filePath.text);
                break;
            }
        }

        return playerControllerFile;

    }

    /// <summary>
    /// Converts the axial bone JSON files from the saved animtion to List<PL2D_AxialBoneFile>
    /// </summary>
    /// <param name="animationName"></param>
    /// <param name="pl2dAnimator"></param>
    /// <returns></returns>
    private static List<PL2D_AxialBoneFile> GetAxialBoneFiles(string animationName, PL2D_Animator pl2dAnimator)
    {
        List<PL2D_AxialBoneFile> axialBoneFiles = new List<PL2D_AxialBoneFile>();

        foreach (TextAsset filePath in Resources.LoadAll("PL2D Animations/" + pl2dAnimator.name + "/" + animationName, typeof(TextAsset)))
        {
            string fileName = filePath.name;
            if (PL2D_Animator.GetTypeFromName(fileName) == "a")
            {
                PL2D_AxialBoneFile axialBoneFile = new PL2D_AxialBoneFile(filePath.text, PL2D_Animator.GetShortName(fileName), PL2D_Animator.GetIndexFromName(fileName), GetLegFiles(animationName, pl2dAnimator).FindAll(f => f.aXialBoneName == PL2D_Animator.GetShortName(fileName)));
                axialBoneFiles.Add(axialBoneFile);
            }

        }

        return axialBoneFiles;

    }

    /// <summary>
    /// converts the leg JSON files from the saved animation to List<PL2D_LegFile> 
    /// </summary>
    /// <param name="animationName"></param>
    /// <param name="pl2dAnimator"></param>
    /// <returns></returns>
    private static List<PL2D_LegFile> GetLegFiles(string animationName, PL2D_Animator pl2dAnimator)
    {

        List<PL2D_LegFile> legFiles = new List<PL2D_LegFile>();

        foreach (TextAsset filePath in Resources.LoadAll("PL2D Animations/" + pl2dAnimator.name + "/" + animationName, typeof(TextAsset)))
        {
            string fileName = filePath.name;
            if (PL2D_Animator.GetTypeFromName(fileName) == "f")
            {
                string[] splittedName = fileName.Split('_');
                PL2D_LegFile legFile = new PL2D_LegFile(filePath.text, PL2D_Animator.GetShortName(splittedName[0]), PL2D_Animator.GetShortName(splittedName[1]), PL2D_Animator.GetIndexFromName(fileName));
                legFiles.Add(legFile);
            }
        }
        return legFiles;

    }
}

/// <summary>
/// Defines an animation file with all the components needed to be applied in a body, the JSON files are converted into an object of this type
/// </summary>
public class PL2D_AnimationFile
{
    public string name;
    public List<PL2D_AxialBoneFile> axialBones;
    public PL2D_PlayerControllerFile playerController;

    public PL2D_AnimationFile(string name, List<PL2D_AxialBoneFile> axialBones, PL2D_PlayerControllerFile playerController)
    {
        this.playerController = playerController;
        this.name = name;
        this.axialBones = axialBones;
    }
}

/// <summary>
/// Interface between JSON files and PL2D_PlayerControllerFile
/// </summary>
public class PL2D_PlayerControllerFile
{
    public string jsonString;

    public PL2D_PlayerControllerFile(string jsonString)
    {
        this.jsonString = jsonString;
    }

    // v1.2 - fix: warnings on creating body parts from json
    // - removed pl2d_AxialBone from PL2D_AxialBoneFile and added method to directly get json values to axial bone
    public void FromJsonToPlayerController(PL2D_AxialBone axialBone)
    {
        if (axialBone.GetComponent<PL2D_PlayerController>() == null)
        {
            PL2D_PlayerController.Initialize(axialBone);
        }

        PL2D_PlayerController pl2d_PlayerController = axialBone.pl2d_PlayerController;

        JSONNode N = JSON.Parse(jsonString);
        pl2d_PlayerController.bodySpeed = N["bodySpeed"].AsFloat;
        pl2d_PlayerController.bodyAcceleration = N["bodyAcceleration"].AsFloat;
        pl2d_PlayerController.enableWallWalk = N["enableWallWalk"].AsBool;
        pl2d_PlayerController.enableJump = N["enableJump"].AsBool;
        pl2d_PlayerController.jumpForce = N["jumpForce"].AsFloat;
        pl2d_PlayerController.horizontalVelocityMultiplierForce = N["horizontalVelocityMultiplierForce"].AsFloat;
        pl2d_PlayerController.preLegBendTime = N["preLegBendTime"].AsFloat;
        pl2d_PlayerController.enableFlipToMouse = N["enableFlipToMouse"].AsBool;
        pl2d_PlayerController.invertFlipToMouse = N["invertFlipToMouse"].AsBool;
        pl2d_PlayerController.pathPoints = JSON_Helper.AsVector3List(N["pathPoints"]);
    }
}

/// <summary>
/// Interface between JSON files and PL2D_AxialBoneFile
/// </summary>
public class PL2D_AxialBoneFile
{
    public string name;
    public int index;
    public List<PL2D_LegFile> legs;
    public string jsonString;

    public PL2D_AxialBoneFile(string jsonString, string name, int index, List<PL2D_LegFile> legs)
    {
        this.name = name;
        this.index = index;
        this.legs = legs;
        this.jsonString = jsonString;
    }

    // v1.2 - fix: warnings on creating body parts from json
    // - removed pl2d_AxialBone from PL2D_AxialBoneFile and added method to directly get json values to axial bone
    public void FromJsonToAxialBone(PL2D_AxialBone axialBone)
    {
        if (axialBone.GetComponent<PL2D_AngleSolver>() == null)
        {
            PL2D_AngleSolver.Initialize(axialBone);
            axialBone.pl2d_AngleSolver.enabled = false;
        }

        JSONNode N = JSON.Parse(jsonString);
        axialBone.autosetBodyDistanceToGround = N["autosetBodyDistanceToGround"].AsBool;
        axialBone.bodyDistanceToGround = N["bodyDistanceToGround"].AsFloat;
        axialBone.autosetRayToGroundLength = N["autosetRayToGroundLength"].AsBool;
        axialBone.RayToGroundLength = N["rayToGroundLength"].AsFloat;
        axialBone.distanceBeforeMoveLeg = N["distanceBeforeMoveLeg"].AsFloat;
        axialBone.angleBeforeMoveLeg = N["angleBeforeMoveLeg"].AsFloat;
        axialBone.bodyStabilizationTopBottomFoot = N["bodyStabilizationTopBottomFoot"].AsFloat;
        axialBone.multiplierlegsDistanceOnVPosCurveVI.keys = JSON_Helper.AsKeyframeArray(N["multiplierlegsDistanceOnVPosCurveVI"]);
        axialBone.angleSolverType = JSON_Helper.AsEnum<PL2D_AxialBone.AngleSolverEnum>(N["angleSolverType"]);
        axialBone.angleTrackSpeed = N["angleTrackSpeed"].AsFloat;
        axialBone.angleSolverEnabled = N["angleSolverEnabled"].AsBool;
        axialBone.playerControllerEnabled = N["playerControllerEnabled"].AsBool;
        axialBone.playerControllerType = JSON_Helper.AsEnum<PL2D_AxialBone.PlayerControllerEnum>(N["playerControllerType"]);
    }
}

/// <summary>
/// Interface between JSON files and PL2D_LegFile
/// </summary>
public class PL2D_LegFile
{
    public string name;
    public string aXialBoneName;
    public int index;
    public string jsonString;

    public PL2D_LegFile(string jsonString, string name, string axialBoneName, int index)
    {
        this.name = name;
        this.aXialBoneName = axialBoneName;
        this.index = index;
        this.jsonString = jsonString;
    }

    // v1.2 - fix: warnings on creating body parts from json
    // - removed pl2d_leg from PL2D_LegFile and added method to directly get json values to leg
    public void FromJsonToLeg(PL2D_Leg leg)
    {
        JSONNode N = JSON.Parse(jsonString);
        leg.stepSpeed = N["stepSpeed"].AsFloat;
        leg.additionalStepLength = N["additionalStepLength"].AsFloat;
        leg.stepHeightCurveSI.keys = JSON_Helper.AsKeyframeArray(N["stepHeightCurveSI"]);
        leg.autoSetTranslationCurve = N["autoSetTranslationCurve"].AsBool;
        leg.stepTranslationCurveSI.keys = JSON_Helper.AsKeyframeArray(N["stepTranslationCurveSI"]);
        leg.phaseShiftFromPreviousStep = N["phaseShiftFromPreviousStep"].AsFloat;
        leg.multiplierSpeedCurveVI.keys = JSON_Helper.AsKeyframeArray(N["multiplierSpeedCurveVI"]);
        leg.additionalStepLengthCurveVI.keys = JSON_Helper.AsKeyframeArray(N["additionalStepLengthCurveVI"]);
        leg.multiplierHeightCurveVI.keys = JSON_Helper.AsKeyframeArray(N["multiplierHeightCurveVI"]);
        leg.footEndings = N["footEndings"].ReadVector2();
        leg.footAngleOffset = N["footAngleOffset"].AsFloat;
        leg.footAngleCurveSI.keys = JSON_Helper.AsKeyframeArray(N["footAngleCurveSI"]);
        leg.multiplierAngleCurveVI.keys = JSON_Helper.AsKeyframeArray(N["multiplierAngleCurveVI"]);
    }
}