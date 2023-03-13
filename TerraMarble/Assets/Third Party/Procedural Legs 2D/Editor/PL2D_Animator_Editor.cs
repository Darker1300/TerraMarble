/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PL2D_Animator))]
[CanEditMultipleObjects]
public class PL2D_Animator_Editor : Editor
{
    PL2D_Animator pl2dAnimator;

    void OnEnable()
    {
        pl2dAnimator = (PL2D_Animator)target;

        EditorApplication.playModeStateChanged += OnEventPlayModeState;
        EditorApplication.hierarchyChanged += OnHierarchyChange;

        pl2dAnimator.OnValidate();
    }

    void OnHierarchyChange()
    {
        if (PrefabUtility.GetPrefabInstanceHandle(pl2dAnimator.gameObject))
        {
            try
            {
                PrefabUtility.UnpackPrefabInstance(pl2dAnimator.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
            catch { }
        }
    }

    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnEventPlayModeState;
        EditorApplication.hierarchyChanged -= OnHierarchyChange;
    }

    private void OnEventPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (pl2dAnimator != null)
            {
                PL2D_SaveSystem.ApplyAnimationToBody(pl2dAnimator, pl2dAnimator.animationsSelectionPopup[pl2dAnimator.animationSelection]);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        DrawInitializerInspector();

        DrawAnimationInspector();

        this.serializedObject.ApplyModifiedProperties();

        // callback similar to OnValidate
        if (GUI.changed)
            pl2dAnimator.OnValidate();
    }

    bool showInitializer = true;

    void DrawInitializerInspector()
    {

        showInitializer = PL2D_EditorHelper.BeginVerticalHeader1Foldout("Initializer", showInitializer);

        if (showInitializer)
        {
            EditorGUI.BeginDisabledGroup(pl2dAnimator.rootBone == null || pl2dAnimator.solverType == PL2D_Animator.SolverTypes.ManuallyAddedSolver && !pl2dAnimator.manualSolverTargetsAdded);
            {
                if (GUILayout.Button("Initialize", GUILayout.Width(100)))
                {
                    pl2dAnimator.Initialize(pl2dAnimator.rootBone, pl2dAnimator.solverType, pl2dAnimator.clearIK, pl2dAnimator.chainLength);
                }
            }
            EditorGUI.EndDisabledGroup();

            pl2dAnimator.rootBone = EditorGUILayout.ObjectField("Root Bone", pl2dAnimator.rootBone, typeof(Transform), true) as Transform;

            if (pl2dAnimator.rootBone)
            {
                pl2dAnimator.solverType = (PL2D_Animator.SolverTypes)EditorGUILayout.EnumPopup("Solver Type", pl2dAnimator.solverType);

                if (pl2dAnimator.solverType == PL2D_Animator.SolverTypes.ManuallyAddedSolver)
                {
                    EditorGUILayout.HelpBox("No IK solver will be added, make sure to add your custom solver to the legs and set each leg solver target", MessageType.Warning);

                    EditorGUILayout.LabelField("Solver Targets");

                    if (pl2dAnimator.customSolverTargets == null || pl2dAnimator.customSolverTargets.Length != pl2dAnimator.allLegsTransforms.Count)
                        pl2dAnimator.customSolverTargets = new Transform[pl2dAnimator.allLegsTransforms.Count];

                    int _legsCount = pl2dAnimator.allLegsTransforms.Count;
                    for (int i = 0; i < _legsCount; i++)
                    {
                        pl2dAnimator.customSolverTargets[i] = EditorGUILayout.ObjectField("    " + PL2D_Animator.GetShortName(pl2dAnimator.allLegsTransforms[i].name),
                                                                                          pl2dAnimator.customSolverTargets[i], typeof(Transform),
                                                                                          true) as Transform;
                    }
                }
                else
                {
                    if (pl2dAnimator.solverType == PL2D_Animator.SolverTypes.ChainSolver)
                    {
                        pl2dAnimator.chainLength = EditorGUILayout.IntField("Chain Length", pl2dAnimator.chainLength);
                    }

                    pl2dAnimator.clearIK = EditorGUILayout.Toggle("Clear IK On Initialize", pl2dAnimator.clearIK);
                    if (pl2dAnimator.clearIK)
                    {
                        if (pl2dAnimator.GetComponent<UnityEngine.U2D.IK.IKManager2D>())
                            EditorGUILayout.HelpBox("On Initialize, this will erase all the previously added solvers on this IK Manager 2D", MessageType.Warning);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Root bone must be assigned", MessageType.Warning);
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    bool showAnimation = true;
    bool showAdvancedSettings = true;

    void DrawAnimationInspector()
    {
        showAnimation = PL2D_EditorHelper.BeginVerticalHeader1Foldout("Animations", showAnimation);
        if (pl2dAnimator.rootBone != null)
        {
            if (pl2dAnimator.axialBones.Count > 0 && pl2dAnimator.animations != null)
            {
                if (pl2dAnimator.selectedAxialBone == null)
                    pl2dAnimator.selectedAxialBone = pl2dAnimator.axialBones[0];

                if (pl2dAnimator.selectedAxialBone.legs.Count > 0)
                {
                    pl2dAnimator.animationSelection = EditorGUILayout.Popup("Animation", pl2dAnimator.animationSelection, pl2dAnimator.animationsSelectionPopup.ToArray());

                    // reset component if reusing the GameObject for a new body 
                    if (pl2dAnimator.animations == null)
                    {
                        pl2dAnimator.Reset();
                    }

                    if (pl2dAnimator.animationSelection == pl2dAnimator.animations.Count - 1)
                    {
                        pl2dAnimator.animationName = EditorGUILayout.TextField("Animation Name", pl2dAnimator.animationName);
                    }

                    if (showAnimation)
                    {
                        PL2D_EditorHelper.HorizontalSeparator();
                        EditorGUILayout.Space();

                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Save", GUILayout.Width(70)))
                        {
                            pl2dAnimator.SaveAnimation(pl2dAnimator.animationName);
                            pl2dAnimator.animationSelection = pl2dAnimator.animationsSelectionPopup.IndexOf(pl2dAnimator.animationName);
                        }

                        if (pl2dAnimator.animationSelection != pl2dAnimator.animations.Count - 1)
                        {
                            EditorGUILayout.Space();
                            if (GUILayout.Button("Delete", GUILayout.Width(70)))
                            {
                                pl2dAnimator.DeleteAnimation(pl2dAnimator.animationsSelectionPopup[pl2dAnimator.animationSelection]);
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();

                        if (pl2dAnimator.animationSelection != pl2dAnimator.animations.Count - 1)
                        {
                            if (pl2dAnimator.animationSelection >= 0)
                            {
                                pl2dAnimator.enableAanimation = EditorGUILayout.Toggle("Enable Animation", pl2dAnimator.enableAanimation);
                            }
                        }

                        showAdvancedSettings = EditorGUILayout.Toggle("Advanced Settings", showAdvancedSettings);

                        if (GUILayout.Button("Apply Default settings", GUILayout.Width(160)))
                        {
                            pl2dAnimator.ApplyDefaultSettings();

                            Debug.Log("Default settings applied!");
                        }
                    }

                    DrawAxialBoneInspector();
                }
                else
                {
                    EditorGUILayout.HelpBox("Animation Not Initialized, missing legs", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Animation Not Initialized, missing axial bones", MessageType.Error);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Root Bone Not Assigned", MessageType.Error);

            if (pl2dAnimator.axialBones.Count > 0)
                pl2dAnimator.axialBones.Clear();
        }

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    void OnSceneGUI()
    {
        if (pl2dAnimator.selectedAxialBone != null)
        {
            PL2D_AxialBone mainAxialBone = pl2dAnimator.selectedAxialBone.mainBoneController;
            float handleSize = mainAxialBone.bodyDistanceToGround / 10;

            Handles.color = Color.green;
            Handles.DrawLine(mainAxialBone.transform.position, mainAxialBone.transform.position + (-mainAxialBone.transform.up * mainAxialBone.RayToGroundLength));
            Handles.color = Color.white;

            Handles.DrawLine(pl2dAnimator.selectedAxialBone.LegsCenter + new Vector3(-handleSize * 2, 0), pl2dAnimator.selectedAxialBone.LegsCenter + new Vector3(handleSize * 2, 0));
            Handles.Label(pl2dAnimator.selectedAxialBone.LegsCenter + new Vector3(-handleSize * 2, 0), "Legs Center", EditorStyles.whiteLabel);

            // v1.2 - refactored
            if (mainAxialBone.pl2d_PlayerController)
            {
                PL2D_PlayerController pl2d_PlayerController = mainAxialBone.pl2d_PlayerController;

                if (pl2d_PlayerController.positionTarget)
                {
                    Handles.RectangleHandleCap(0, pl2d_PlayerController.positionTarget.position, Quaternion.Euler(mainAxialBone.MoveDirection), handleSize, EventType.Repaint);
                    Handles.Label(pl2d_PlayerController.positionTarget.position, "Target", EditorStyles.whiteLabel);
                }

                if (mainAxialBone.playerControllerType == PL2D_AxialBone.PlayerControllerEnum.PathFollow)
                {
                    if (pl2d_PlayerController.pathPoints.Count > 0)
                    {
                        int _pointsCount = pl2d_PlayerController.pathPoints.Count;
                        Color _yellow = Color.yellow;
                        Color _white = Color.white;
                        for (int i = 0; i < _pointsCount; i++)
                        {
                            if (i > 0)
                            {
                                Handles.color = _yellow;
                                Handles.DrawLine(pl2d_PlayerController.pathPoints[i - 1], pl2d_PlayerController.pathPoints[i]);
                                Handles.color = _white;
                            }

                            pl2d_PlayerController.pathPoints[i] = Handles.PositionHandle(pl2d_PlayerController.pathPoints[i], Quaternion.identity);
                            Handles.Label(pl2d_PlayerController.pathPoints[i], "Path Point " + i, EditorStyles.whiteLabel);
                        }
                    }
                }
            }
        }
    }

    void DrawAxialBoneValues(PL2D_AxialBone axialBone)
    {
        PL2D_EditorHelper.LabelBold("Axial Bone Movement");
        {
            if (showAdvancedSettings)
            {
                EditorGUI.BeginDisabledGroup(!axialBone.isMainAxialBone);
                {
                    axialBone.autosetBodyDistanceToGround = EditorGUILayout.Toggle("AutoSet Body Distance to Ground", axialBone.autosetBodyDistanceToGround);

                    EditorGUI.BeginDisabledGroup(axialBone.autosetBodyDistanceToGround);
                    {
                        axialBone.bodyDistanceToGround = EditorGUILayout.FloatField("Body Distance to Ground", axialBone.bodyDistanceToGround);
                    }
                    EditorGUI.EndDisabledGroup();

                    axialBone.autosetRayToGroundLength = EditorGUILayout.Toggle("AutoSet Ray to Ground Length", axialBone.autosetRayToGroundLength);

                    EditorGUI.BeginDisabledGroup(axialBone.autosetRayToGroundLength);
                    {
                        axialBone.rayToGroundLength = EditorGUILayout.FloatField("Ray to Ground Length", axialBone.RayToGroundLength);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.EndDisabledGroup();
            }

            axialBone.distanceBeforeMoveLeg = EditorGUILayout.FloatField("Distance Before Move Leg", axialBone.distanceBeforeMoveLeg);
            axialBone.angleBeforeMoveLeg = EditorGUILayout.FloatField("Angle Before Move Leg", axialBone.angleBeforeMoveLeg);

            // v1.1 - Body Stabilization hide on Advanced Settings == false
            if (showAdvancedSettings)
            {
                EditorGUILayout.LabelField("Body Stabilization");
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("< Top Foot");
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Bottom Foot >", GUILayout.Width(130));
                }
                EditorGUILayout.EndHorizontal();
                axialBone.bodyStabilizationTopBottomFoot = EditorGUILayout.Slider(axialBone.bodyStabilizationTopBottomFoot, 0, 1);
                EditorGUILayout.Space();


                EditorGUILayout.LabelField("Evaluates Speed Progression", EditorStyles.centeredGreyMiniLabel);

                axialBone.multiplierlegsDistanceOnVPosCurveVI = EditorGUILayout.CurveField("Vertical Offset Multiplier", axialBone.multiplierlegsDistanceOnVPosCurveVI);
            }
        }

        PL2D_EditorHelper.HorizontalSeparator();
        PL2D_EditorHelper.LabelBold("Angle Solver");
        {
            pl2dAnimator.selectedAxialBone.angleSolverEnabled = EditorGUILayout.Toggle("Enable", pl2dAnimator.selectedAxialBone.angleSolverEnabled);
            if (pl2dAnimator.selectedAxialBone.angleSolverEnabled)
            {
                axialBone.angleSolverType = (PL2D_AxialBone.AngleSolverEnum)EditorGUILayout.EnumPopup("Solver Type", axialBone.angleSolverType);

                if (axialBone.angleSolverType == PL2D_AxialBone.AngleSolverEnum.RelativeToLegs && axialBone.pl2d_AngleSolver == null)
                {
                    EditorGUILayout.HelpBox("Angle Solver not initialized, check if you have the Axial Bone set correctly", MessageType.Error);
                }

                axialBone.angleTrackSpeed = EditorGUILayout.FloatField("Rotation Speed", axialBone.angleTrackSpeed);
            }
        }
        PL2D_EditorHelper.HorizontalSeparator();

// v1.2 - refactored 
        PL2D_EditorHelper.LabelBold("Player Controller");
        {
            EditorGUI.BeginDisabledGroup(!axialBone.isMainAxialBone);
            {
                axialBone.mainBoneController.playerControllerEnabled = EditorGUILayout.Toggle("Enable", axialBone.mainBoneController.playerControllerEnabled);
                if (axialBone.playerControllerEnabled)
                {
                    if (axialBone.mainBoneController.pl2d_PlayerController == null)
                        PL2D_PlayerController.Initialize(axialBone.mainBoneController);

                    PL2D_PlayerController pl2d_PlayerController = axialBone.mainBoneController.pl2d_PlayerController;

                    axialBone.mainBoneController.playerControllerType = (PL2D_AxialBone.PlayerControllerEnum)EditorGUILayout.EnumPopup("Controller Type", axialBone.mainBoneController.playerControllerType);

                    pl2d_PlayerController.bodySpeed = EditorGUILayout.FloatField("Body Speed", pl2d_PlayerController.bodySpeed);

                    if (axialBone.mainBoneController.playerControllerType == PL2D_AxialBone.PlayerControllerEnum.KeyboardInput)
                    {
                        pl2d_PlayerController.bodyAcceleration = EditorGUILayout.FloatField("Body Acceleration", pl2d_PlayerController.bodyAcceleration);
                        pl2d_PlayerController.enableWallWalk = EditorGUILayout.Toggle("Wall Walk", pl2d_PlayerController.enableWallWalk);
                        pl2d_PlayerController.enableFlipToMouse = EditorGUILayout.Toggle("Flip To Mouse", pl2d_PlayerController.enableFlipToMouse);
                        EditorGUI.BeginDisabledGroup(!pl2d_PlayerController.enableFlipToMouse);
                        {
                            pl2d_PlayerController.invertFlipToMouse = EditorGUILayout.Toggle("Invert Flip", pl2d_PlayerController.invertFlipToMouse);
                        }
                        EditorGUI.EndDisabledGroup();
                        pl2d_PlayerController.enableJump = EditorGUILayout.Toggle("Jump", pl2d_PlayerController.enableJump);
                        EditorGUI.BeginDisabledGroup(!pl2d_PlayerController.enableJump);
                        {
                            pl2d_PlayerController.jumpForce = EditorGUILayout.FloatField("Jump Force", pl2d_PlayerController.jumpForce);
                            float jumpForce = pl2d_PlayerController.jumpForce;
                            float gravityY = Physics2D.gravity.y;
                            if(jumpForce <= -gravityY)
                            {
                                EditorGUILayout.HelpBox(string.Format("Jump Force may be too small \nJump Force ({0}) <= -Gravit Y ({1})", jumpForce, gravityY), MessageType.Warning);
                            }
                            pl2d_PlayerController.horizontalVelocityMultiplierForce = EditorGUILayout.FloatField("Horizontal Velocity Multiplier Force", pl2d_PlayerController.horizontalVelocityMultiplierForce);
                            pl2d_PlayerController.preLegBendTime = EditorGUILayout.FloatField("Pre Leg Bend Time", pl2d_PlayerController.preLegBendTime);
                        }
                        EditorGUI.EndDisabledGroup();
                    }

                    if (axialBone.mainBoneController.playerControllerType == PL2D_AxialBone.PlayerControllerEnum.PathFollow)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal();

                        if (GUILayout.Button("Add Point", GUILayout.Width(100)))
                        {
                            pl2d_PlayerController.AddPathPoint(axialBone.mainBoneController.transform.position, pl2d_PlayerController.pathPoints.Count);
                        }
                        EditorGUILayout.Space();
                        if (GUILayout.Button("Remove Point", GUILayout.Width(100)))
                        {
                            pl2d_PlayerController.RemovePathPoint(pl2d_PlayerController.pathPoints.Count - 1);
                        }

                        EditorGUILayout.EndHorizontal();

                        if (pl2d_PlayerController.pathPoints == null)
                        {
                            pl2d_PlayerController.pathPoints = new List<Vector3>();
                        }

                        int _pointsCount = pl2d_PlayerController.pathPoints.Count;
                        for (int i = 0; i < _pointsCount; i++)
                        {
                            pl2d_PlayerController.pathPoints[i] = EditorGUILayout.Vector3Field("Point " + i,
                                                                            pl2d_PlayerController.pathPoints[i]);
                        }

                        EditorGUILayout.Space();

                        EditorGUI.BeginDisabledGroup(pl2d_PlayerController.pathPoints.Count <= 0);
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Play Path Follow", GUILayout.Width(120)))
                            {
                                pl2d_PlayerController.PlayPathFollow();
                            }
                            EditorGUILayout.Space();
                            if (GUILayout.Button("Pause Path Follow", GUILayout.Width(120)))
                            {
                                pl2d_PlayerController.PausePathFollow();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    bool showAxialBone = true;

    void DrawAxialBoneInspector()
    {
        showAxialBone = PL2D_EditorHelper.BeginVerticalHeader2Foldout("Axial Bones", showAxialBone);

        pl2dAnimator.axialBoneSelection = EditorGUILayout.Popup("Axial Bone", pl2dAnimator.axialBoneSelection, pl2dAnimator.axialBonesSelectionPopup);

        if (showAxialBone)
        {
            PL2D_EditorHelper.HorizontalSeparator();
            EditorGUILayout.Space();

            if (GUILayout.Button("Override All Axial Bones", GUILayout.Width(160)))
            {
                pl2dAnimator.OverrideAllAxialBones(pl2dAnimator.selectedAxialBone);
            }

            PL2D_EditorHelper.HorizontalSeparator();

            DrawAxialBoneValues(pl2dAnimator.selectedAxialBone);
        }

        DrawLegInspector(pl2dAnimator.selectedAxialBone);

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    void DrawLegValues(PL2D_Leg leg)
    {
        PL2D_EditorHelper.LabelBold("Leg Movement");
        {
            leg.stepSpeed = EditorGUILayout.FloatField("Step Speed", leg.stepSpeed);

            if (showAdvancedSettings)
            {
                leg.additionalStepLength = EditorGUILayout.FloatField("Additional Speed Legth", leg.additionalStepLength);
            }

            EditorGUILayout.LabelField("Evaluates Step Translation Progression", EditorStyles.centeredGreyMiniLabel);

            if (pl2dAnimator.selectedLeg.stepHeightCurveSI.length <= 0 || pl2dAnimator.selectedLeg.stepTranslationCurveSI.length <= 0)
            {
                EditorGUILayout.HelpBox(" Show Step Curves not available yet. Height and Translation Curves need to be set first", MessageType.Warning);
                showStepCurves = false;
            }

            EditorGUI.BeginDisabledGroup(pl2dAnimator.selectedLeg.stepHeightCurveSI.length <= 0 || pl2dAnimator.selectedLeg.stepTranslationCurveSI.length <= 0);
            {
                showStepCurves = EditorGUILayout.Toggle("Show Step Curves", showStepCurves);
            }
            EditorGUI.EndDisabledGroup();

            if (showStepCurves)
            {
                DrawStepCurves();
            }

            leg.stepHeightCurveSI = EditorGUILayout.CurveField("Step Height", leg.stepHeightCurveSI);

            if (showAdvancedSettings)
            {
                leg.autoSetTranslationCurve = EditorGUILayout.Toggle("AutoSet Translation Curve", leg.autoSetTranslationCurve);

                leg.stepTranslationCurveSI = EditorGUILayout.CurveField("Step Translation", leg.stepTranslationCurveSI);
            }

            leg.phaseShiftFromPreviousStep = EditorGUILayout.Slider("Phase Shift From Previous Step", leg.phaseShiftFromPreviousStep, 0, 1);

            if (showAdvancedSettings)
            {
                EditorGUILayout.LabelField("Evaluates Speed Progression", EditorStyles.centeredGreyMiniLabel);

                leg.multiplierSpeedCurveVI = EditorGUILayout.CurveField("Step Speed Multiplier", leg.multiplierSpeedCurveVI);
                leg.additionalStepLengthCurveVI = EditorGUILayout.CurveField("Step Length", leg.additionalStepLengthCurveVI);
                leg.multiplierHeightCurveVI = EditorGUILayout.CurveField("Step Height Multiplier", leg.multiplierHeightCurveVI);
            }
        }

        if (showAdvancedSettings)
        {
            PL2D_EditorHelper.HorizontalSeparator();

            PL2D_EditorHelper.LabelBold("Foot Movement");
            {
                EditorGUILayout.LabelField("Foot Endings");
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("< Left");
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Right >", GUILayout.Width(50));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    leg.footEndings.x = EditorGUILayout.FloatField(leg.footEndings.x);
                    EditorGUILayout.Space();
                    leg.footEndings.y = EditorGUILayout.FloatField(leg.footEndings.y);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                leg.footAngleOffset = EditorGUILayout.FloatField("Foot Angle Offset", leg.footAngleOffset);

                EditorGUILayout.LabelField("Evaluate Step Translation Progression", EditorStyles.centeredGreyMiniLabel);
                leg.footAngleCurveSI = EditorGUILayout.CurveField("Foot Angle", leg.footAngleCurveSI);

                EditorGUILayout.LabelField("Evaluate Speed Progression", EditorStyles.centeredGreyMiniLabel);
                leg.multiplierAngleCurveVI = EditorGUILayout.CurveField("Foot Angle Multiplier", leg.multiplierAngleCurveVI);
            }

            PL2D_EditorHelper.HorizontalSeparator();
        }
    }

    Vector3[] pointsHeight;
    Vector3[] pointsTranslation;

    float ScaledValue(float x, float a, float b, float max, float min)
    {
        float value = 0;
        value = (((b - a) * (x - min)) / (max - min)) + a;

        return value;
    }

    Keyframe GetMaxValueKeyframe(Keyframe[] keys)
    {
        Keyframe maxKey = new Keyframe(0, 0);
        float _maxKeyValue = maxKey.value;
        foreach (Keyframe key in keys)
        {
            if (key.value > _maxKeyValue)
            {
                maxKey = key;
            }
        }

        return maxKey;
    }

    float GetCurveMaxValue(AnimationCurve curve)
    {
        float maxValue = 0;

        float len = pl2dAnimator.selectedLeg.stepHeightCurveSI.keys[pl2dAnimator.selectedLeg.stepHeightCurveSI.length - 1].time;
        for (float i = 0; i < len; i += len / 10)
        {
            float evaluatedValue = curve.Evaluate(i);
            if (evaluatedValue > maxValue)
            {
                maxValue = evaluatedValue;
            }
        }

        return maxValue;
    }

    void DrawStepCurves()
    {
        var rect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("", GUILayout.Height(80));

        float colorFloat = 1f / pl2dAnimator.selectedAxialBone.legs.Count;
        float xSize = ((pl2dAnimator.selectedAxialBone.legs.Count - 1) * pl2dAnimator.selectedLeg.stepHeightCurveSI.keys[pl2dAnimator.selectedLeg.stepHeightCurveSI.length - 1].time) + pl2dAnimator.selectedLeg.phaseShiftFromPreviousStep;
        float ySize = GetCurveMaxValue(pl2dAnimator.selectedLeg.stepHeightCurveSI) * pl2dAnimator.selectedLeg.multiplierHeightCurveVI.Evaluate(1);

        float padding = 20;
        float _rectxMax = rect.xMax;
        float _rectxMin = rect.xMin;
        float _rectyMax = rect.yMax;
        float _rectyMin = rect.yMin;
        foreach (PL2D_Leg leg in pl2dAnimator.selectedAxialBone.legs)
        {
            AnimationCurve _legStepHeightCurveSI = leg.stepHeightCurveSI;
            AnimationCurve _legMultiplierHeightCurveVI = leg.multiplierHeightCurveVI;

            int xScale = 0;
            float yScale = 0;

            pointsHeight = new Vector3[11];
            pointsTranslation = new Vector3[11];
            float _legPhaseShiftFromPreviousStep = leg.phaseShiftFromPreviousStep;
            int _legIndex = leg.legIndex;
            for (int i = 0; i <= 10; i++)
            {
                float xUnscaledHeight = (((float)i / 10) + (_legPhaseShiftFromPreviousStep * _legIndex));
                float yUnscaledHeight = (_legStepHeightCurveSI.Evaluate((float)i / 10) * _legMultiplierHeightCurveVI.Evaluate(1));
                float xScaledHeight = ScaledValue(xUnscaledHeight, _rectxMax - padding, _rectxMin + padding + 25, 0, xSize);
                float yScaledHeight = ScaledValue(yUnscaledHeight, _rectyMin + padding, _rectyMax - padding, 0, ySize);

                pointsHeight[i] = new Vector3(xScaledHeight, yScaledHeight, 0);

                if (pointsHeight[i].x >= _rectxMax)
                {
                    break;
                }

                if (leg == pl2dAnimator.selectedLeg)
                {
                    if (i - xScale >= 2f)
                    {
                        xScale = i;
                        Handles.Label(new Vector3(pointsHeight[i].x, _rectyMax - padding), xUnscaledHeight.ToString("F1"), EditorStyles.miniLabel);

                        Handles.color = new Color(0.7f, 0.7f, 0.7f);
                        Handles.DrawLine(new Vector3(pointsHeight[i].x, _rectyMax - padding), new Vector3(pointsHeight[i].x, pointsHeight[i].y));
                    }

                    if (yUnscaledHeight > yScale)
                    {
                        yScale = yUnscaledHeight;
                        Handles.Label(new Vector3(_rectxMin + padding + 10, pointsHeight[i].y - 10), yUnscaledHeight.ToString("F1"), EditorStyles.miniLabel);

                        Handles.color = new Color(0.7f, 0.7f, 0.7f);
                        Handles.DrawLine(new Vector3(_rectxMin + padding + 10, pointsHeight[i].y), new Vector3(pointsHeight[i].x, pointsHeight[i].y));
                    }
                }

                float yUnscaledTranslation = (leg.stepTranslationCurveSI.Evaluate((float)i / 10));
                float yScaledTranslation = ScaledValue(yUnscaledTranslation, _rectyMin + padding, _rectyMax - padding, 0, 1);

                pointsTranslation[i] = new Vector3(xScaledHeight, yScaledTranslation, 0);

                if (i > 0 && yUnscaledTranslation > 0)
                {
                    Handles.color = new Color(colorFloat * leg.legIndex, colorFloat * leg.legIndex, 1);
                    Handles.DrawLine(pointsTranslation[i - 1], pointsTranslation[i]);
                }

                if (i > 0)
                {
                    Handles.color = new Color(1, colorFloat * leg.legIndex, colorFloat * leg.legIndex);
                    Handles.DrawLine(pointsHeight[i - 1], pointsHeight[i]);
                }
            }
        }

        Vector2 pos = rect.center;
        EditorGUIUtility.RotateAroundPivot(-90, pos);
        GUIStyle s = new GUIStyle(EditorStyles.centeredGreyMiniLabel);

        s.normal.textColor = new Color(1, colorFloat * pl2dAnimator.selectedLeg.legIndex, colorFloat * pl2dAnimator.selectedLeg.legIndex);
        EditorGUI.LabelField(new Rect(rect.xMin, rect.yMin - (rect.width / 2) + 10, rect.width, rect.height), "Height", s);
        Handles.DrawLine(new Vector3(rect.xMin, rect.yMin + 10), new Vector3(rect.xMin, rect.yMin));

        s.normal.textColor = new Color(colorFloat * pl2dAnimator.selectedLeg.legIndex, colorFloat * pl2dAnimator.selectedLeg.legIndex, 1);
        EditorGUI.LabelField(new Rect(rect.xMin, rect.yMin - (rect.width / 2) + 20, rect.width, rect.height), "Translation", s);
        EditorGUIUtility.RotateAroundPivot(90, pos);

        s.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1);
        GUI.color = new Color(1, 1, 1);
        EditorGUI.LabelField(new Rect(rect.xMin, rect.yMin - (rect.height / 2) + 10, rect.width, rect.height), "Time", s);

        EditorGUILayout.EndVertical();
        Handles.color = Color.gray;

        EditorGUILayout.Space();
    }

    bool showLeg = true;
    bool showStepCurves = true;

    void DrawLegInspector(PL2D_AxialBone selectedAxialBone)
    {
        showLeg = PL2D_EditorHelper.BeginVerticalHeader3Foldout("Legs", showLeg);

        pl2dAnimator.legsSelection = EditorGUILayout.Popup("Leg", pl2dAnimator.legsSelection, pl2dAnimator.legsSelectionPopup);

        if (showLeg && pl2dAnimator.selectedLeg != null)
        {
            PL2D_EditorHelper.HorizontalSeparator();
            EditorGUILayout.Space();

            if (GUILayout.Button("Override All Legs in Axial Bone", GUILayout.Width(190)))
            {
                pl2dAnimator.OverrideAllLegsInAxialBone(pl2dAnimator.selectedLeg);
            }

            if (GUILayout.Button("Override All Legs in Body", GUILayout.Width(160)))
            {
                pl2dAnimator.OverrideAllLegsInBody(pl2dAnimator.selectedLeg);
            }

            PL2D_EditorHelper.HorizontalSeparator();

            DrawLegValues(pl2dAnimator.selectedLeg);
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }
}

