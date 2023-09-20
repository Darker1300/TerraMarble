using MathUtility;
using NaughtyAttributes;
using Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static UnityUtility.TweenUtility;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Region : MonoBehaviour
{
    public class RegionHitInfo
    {
        public Collision2D collision = null;
        public BallStateTracker ballState = null;
        public Region region = null;
        public SurfaceObject surfaceObj = null;
    }

    public enum RegionID
    {
        NULL = -1,
        Water,
        Rock,
        Sand,
        Dirt,
        Grass,
        Forest,
        Mountain,
        Volcano,
        Men,
        SIZE
    }



    [Header("Config")] public RegionID regionID = RegionID.Water;
    public AnimCurve animTerraform = new();

    [Header("Debug")] public List<SurfaceObject> surfaceObjects = new();
    public List<EntityObject> entityChildren = new();
    public List<EntityObject> entitysOnRegion = new();

    [SerializeField] private RegionID targetID = RegionID.Water;
    private int _regionIndex = -1;

    [SerializeField] private Transform _base = null;
    private Wheel _wheel = null;
    private Disc _regionDisc = null;
    private PolygonCollider2D _regionCollider = null;

    private readonly string defaultBaseName = "Base";
    private readonly Vector2 defaultBasePosition = new(0.5f, 0f);

    [HideInInspector] public UnityEvent<RegionHitInfo> BallHitEnter = new();
    [HideInInspector] public UnityEvent<RegionHitInfo> BallHitExit = new();

    #region References Properties

    public Disc RegionDisc
    {
        get
        {
            if (!_regionDisc) _regionDisc = GetComponent<Disc>();
            return _regionDisc;
        }
        set => _regionDisc = value;
    }

    public Transform Base
    {
        get => TryCreateBase();
        set => _base = value;
    }

    public Wheel Wheel
    {
        get => _wheel == null
            ? _wheel = FindObjectOfType<Wheel>()
            : _wheel;
        set => _wheel = value;
    }

    public WheelRegionsManager RegionsMan
        => Wheel.regions;

    public Region RegionTemplate
        => Wheel.regions.RegionTemplate;

    public PolygonCollider2D RegionCollider
    {
        get => _regionCollider == null
            ? _regionCollider = Base.GetComponent<PolygonCollider2D>()
            : _regionCollider;
        set => _regionCollider = value;
    }

    #endregion

    #region Properties

    public float Thickness
    {
        get
        {
            if (RegionDisc.ThicknessSpace != ThicknessSpace.Meters)
                RegionDisc.ThicknessSpace = ThicknessSpace.Meters;
            return RegionDisc.Thickness;
        }
        set
        {
            if (RegionDisc.ThicknessSpace != ThicknessSpace.Meters)
                RegionDisc.ThicknessSpace = ThicknessSpace.Meters;
            RegionDisc.Thickness = value;
        }
    }

    public float ThicknessHalf
        => Thickness * 0.5f;

    public float RadiusBase
    {
        get
        {
            if (RegionDisc.RadiusSpace != ThicknessSpace.Meters)
                RegionDisc.RadiusSpace = ThicknessSpace.Meters;
            return RegionDisc.Radius - ThicknessHalf;
        }
    }

    public float RadiusFull
        => RadiusBase + Thickness;

    public float AngleCenter
        => Mathf.LerpAngle(AngleStart, AngleEnd, 0.5f);

    public float AngleStart
        => RegionDisc.AngRadiansStart * Mathf.Rad2Deg;

    public float AngleEnd
        => RegionDisc.AngRadiansEnd * Mathf.Rad2Deg;

    public float AngleSize
        => Mathf.DeltaAngle(AngleStart, AngleEnd);

    public Vector2 AngleCenterVector
        => MathU.DegreesToVector2(AngleCenter);

    public int RegionIndex
    {
        get
        {
            if (_regionIndex == -1)
            {
                int id = this.GetInstanceID();
                _regionIndex = Array.FindIndex(RegionsMan.Regions, r => r.GetInstanceID() == id);
            }
            return _regionIndex;
        }
    }

    #endregion

    #region Events


    private void Start()
    {
        RegionDisc ??= GetComponent<Disc>();
        targetID = regionID;
        surfaceObjects = GetComponentsInChildren<SurfaceObject>().ToList();

        SceneManager.sceneUnloaded += scene => animTerraform.Stop();

        SetRegionCollider(regionID == RegionID.Water ? 0f : 1f);
    }

    private void Update()
    {
        animTerraform.Update();
    }

    private void OnDrawGizmosSelected()
    {
        if (!RegionsMan.RegionTemplateIsNull)
            Gizmos.DrawWireSphere(
                transform.position + transform.TransformVector(
                    (Vector3)MathU.LerpAngleUnclamped(AngleStart, AngleEnd, defaultBasePosition.x)
                        .DegreesToVector2()
                    * (RegionTemplate.RadiusBase + Mathf.LerpUnclamped(0f, RegionTemplate.Thickness, defaultBasePosition.y))
                ),
                transform.lossyScale.magnitude * 0.05f);
    }

    #endregion

    #region Base Transform Functions

    private Transform ConfigureBase()
    {
        RegionDisc ??= GetComponent<Disc>();

        Vector2 centerVector = AngleCenterVector;
        Vector3 angleVector = (Vector3)centerVector * RegionTemplate.RadiusBase;

        _base.position = transform.position
                         + transform.TransformVector(
                             angleVector);
        _base.up = transform.TransformVector(centerVector);
        _regionCollider = Base.GetComponent<PolygonCollider2D>();
        return _base;
    }

    [Button]
    public Transform TryCreateBase()
    {
        if (_base == null)
        {
            var gen = FindObjectOfType<WheelGenerator>();
            _base = Instantiate(gen.pregenBase, transform, false).transform;
            _base.gameObject.name = defaultBaseName;
            //_base = new GameObject(defaultBaseName).transform;
            //_base.SetParent(transform, false);
            ConfigureBase();
        }

        return _base;
    }

    public Transform ResetBase()
    {
        if (_base == null) return TryCreateBase();
        return ConfigureBase();
    }

    public void FindBase()
    {
        if (_base == null) _base = transform.Find(defaultBaseName);
    }

    #endregion

    #region Terraform Functions

    public void TerraformRegion(RegionID _targetID)
    {
        if (!Application.isPlaying) return;

        if (targetID == _targetID ||
            _targetID == regionID ||
            animTerraform.Animating)
            return;

        targetID = _targetID;

        animTerraform.Reset();
        animTerraform.Play();

        UnityAction<float> updateAction = value => { };
        UnityAction<float> finishAction = value => { };

        bool targetIsWater = _targetID == RegionID.Water;
        if (targetIsWater || regionID == RegionID.Water)
        {
            if (targetIsWater) // Land to Water
                updateAction += UpdateLandToWater;
            else // Water to Land
                updateAction += UpdateWaterToLand;
        }

        // Set Color
        updateAction += UpdateRegionColor;

        RegionConfig goalConfig = RegionsMan.configs[targetID];
        GameObject goalPrefab = goalConfig.SurfacePrefab;
        bool goalBlankTag = goalPrefab == null || goalPrefab.CompareTag("Untagged");

        // Destroy inappropriate surface objects
        for (int index = 0; index < surfaceObjects.Count; index++)
        {
            SurfaceObject surfaceObject = surfaceObjects[index];
            if (goalPrefab == null)
                surfaceObjects[index].DoDestroy();
            // if goal is untagged or objects do not match goal objects
            else if ((goalBlankTag || !surfaceObject.gameObject.CompareTag(goalPrefab.tag)) &&
                     !surfaceObject.isDestroyed)
                surfaceObjects[index].DoDestroy();
        }

        // Trim list of dying objects
        surfaceObjects = surfaceObjects
            .Where(growObj => growObj != null && !growObj.isDestroyed)
            .ToList();

        // Spawn new surface obj
        if (goalPrefab != null)
        {
            GameObject newSurfaceObj = Instantiate(goalPrefab, Base, false);
            SurfaceObject newGrowable = newSurfaceObj.GetComponentInChildren<SurfaceObject>(true);
            surfaceObjects.Add(newGrowable);
        }

        finishAction += value =>
        {
            // Remove Update action
            animTerraform.Updated.RemoveListener(updateAction);
            // Set new RegionID
            regionID = targetID;

            animTerraform.Finished.RemoveListener(finishAction);
        };

        animTerraform.Updated.AddListener(updateAction);
        animTerraform.Finished.AddListener(finishAction);
    }

    private void UpdateRegionColor(float value)
    {
        RegionDisc.Color = Color.Lerp(RegionsMan.configs[regionID].RegionColor, RegionsMan.configs[targetID].RegionColor, value);
    }

    private void UpdateWaterToLand(float value)
    {
        Thickness = Mathf.Lerp(0f, RegionTemplate.Thickness, value);
        RegionCollider.offset = Vector2.down * (RegionTemplate.Thickness - RegionDisc.Thickness);
        SetRegionCollider(value);
    }

    private void UpdateLandToWater(float value)
    {
        Thickness = Mathf.Lerp(RegionTemplate.Thickness, 0f, value);
        SetRegionCollider(value);
    }

    #endregion

    /// <param name="state">0..1f</param>
    public void SetRegionCollider(float state = 1f)
    {
        if (RegionCollider != null && !RegionsMan.RegionTemplateIsNull)
        {
            RegionCollider.offset = Vector2.down *
                                    Mathf.Lerp(RegionTemplate.Thickness - RegionDisc.Thickness,
                                        RegionTemplate.Thickness - Thickness,
                                        state);
        }

    }

    [Button]
    public void TerraformToWater()
    {
        TerraformRegion(RegionID.Water);
    }

    [Button]
    public void TerraformToDirt()
    {
        TerraformRegion(RegionID.Dirt);
    }

    [Button]
    public void TerraformToForest()
    {
        TerraformRegion(RegionID.Forest);
    }

    [Button]
    public void Grow()
    {
        foreach (SurfaceObject surfaceObject in surfaceObjects)
        {
            Growable growable = surfaceObject.GetComponent<Growable>();
            if (growable != null) growable.TryGrowState();
        }
    }

    [Button]
    public void Reset()
    {
        foreach (SurfaceObject surfaceObject in surfaceObjects)
        {
            Growable growable = surfaceObject.GetComponent<Growable>();
            if (growable != null) growable.ResetState();
        }
    }

    public Growable FindGrow()
    {
        foreach (SurfaceObject surfaceObject in surfaceObjects)
        {
            Growable growable = surfaceObject.GetComponent<Growable>();
            if (growable != null) return growable;
        }
        return null;
    }

    #region Spatial Helpers

    /// <param name="_x">Percentage of region</param>
    /// <param name="_y">Percentage</param>
    /// <returns>World position</returns>
    public Vector2 RegionPosition(float _x, float _y = 1f)
    {
        return transform.position + transform.TransformVector(
            (Vector3)MathU.DegreesToVector2(
                MathU.LerpAngleUnclamped(AngleStart, AngleEnd, _x))
            * (RadiusBase + Mathf.LerpUnclamped(0f, Thickness, _y))
        );
    }

    /// <returns>World position</returns>
    public Vector2 RegionPosition(Vector2 _regionPosition)
    {
        return RegionPosition(_regionPosition.x, _regionPosition.y);
    }

    /// <returns>Returns Regions array index + 0..1</returns>
    public float WorldToRegionDistance(Vector2 _worldPos)
    {
        Vector2 localPos = transform.InverseTransformPoint(_worldPos);
        var angle = localPos.normalized.ToDegrees();
        var segments = angle * (1f / Mathf.DeltaAngle(AngleStart, AngleEnd));
        return segments;
    }

    /// <returns>Returns Regions array index + 0..1</returns>
    public int WorldToRegionIndex(Vector2 _worldPos)
    {
        var distance = WorldToRegionDistance(_worldPos);
        var repeat = Mathf.Repeat(distance, Wheel.regions.RegionCount - 0.9999f);
        return (int)Mathf.Floor(repeat);
    }

    /// <returns> X: Regions array index + 0..1;
    /// Y: typically 0..1, this corresponds to multiples of Thickness, starting from the Base radius.</returns>
    public Vector2 WorldToRegion(Vector2 _worldPos)
    {
        Vector2 localPos = transform.InverseTransformPoint(_worldPos);
        var angle = localPos.normalized.ToDegrees();
        var segments = angle * (1f / Mathf.DeltaAngle(AngleStart, AngleEnd));

        var totalDst = Vector2.Distance(transform.position, localPos);
        var height = (totalDst - RadiusBase) * (1f / Thickness);

        return new Vector2(segments, height);
    }

    public int GetAdjacentRegionIndex(int indexIncrement)
    {
        int result = RegionIndex;
        return MathU.Repeat(
            result + indexIncrement,
            0,
            RegionsMan.RegionCount - 1);
    }

    public Region GetAdjacentRegion(int indexIncrement)
    {
        int result = GetAdjacentRegionIndex(indexIncrement);
        return RegionsMan[result];
    }

    #endregion
}