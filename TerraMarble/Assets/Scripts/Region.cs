using System;
using System.Collections.Generic;
using System.Linq;
using MathUtility;
using NaughtyAttributes;
using Shapes;
using UnityEngine;
using UnityEngine.Events;
using static UnityUtility.Tween;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Region : MonoBehaviour
{
    public class RegionHitInfo
    {
        public Collision2D collision = null;
        public BallStateTracker ballState = null;
        public Region region = null;
        public GameObject surfaceObj = null;
    }

    public enum RegionID
    {
        Water,
        Rock,
        Sand,
        Dirt,
        Grass,
        Forest,
        SIZE
    }

    [Header("Config")]
    public AnimCurve animTerraform = new();
    public RegionID regionID = RegionID.Water;

    [HideInInspector] public UnityEvent<RegionHitInfo> BallHitEnter = new();
    [HideInInspector] public UnityEvent<RegionHitInfo> BallHitExit = new();

    [Header("Debug")]
    public List<Growable> surfaceObjects = new();
    [SerializeField] private RegionID targetID = RegionID.Water;
    [SerializeField] private Transform _base = null;

    private Wheel _wheel = null;
    private Disc _regionDisc = null;
    private PolygonCollider2D _regionCollider = null;
    private readonly string defaultBaseName = "Base";
    private readonly Vector2 defaultBasePosition = new(0.5f, 0f);

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
        => TryCreateBase();

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
        => MathU.DegreeToVector2(AngleCenter);

    #endregion

    #region Events

    private void Update()
    {
        animTerraform.Update();
    }

    private void Awake()
    {
        RegionDisc ??= GetComponent<Disc>();
        targetID = regionID;
        surfaceObjects = GetComponentsInChildren<Growable>().ToList();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position + transform.TransformVector(
                (Vector3)MathU.DegreeToVector2(MathU.LerpAngleUnclamped(AngleStart, AngleEnd, defaultBasePosition.x))
                * (RegionTemplate.RadiusBase + Mathf.LerpUnclamped(0f, RegionTemplate.Thickness, defaultBasePosition.y))
            ),
            transform.lossyScale.magnitude * 0.05f);
    }

    #endregion

    #region Base Transform Functions

    private Transform InitBase()
    {
        RegionDisc ??= GetComponent<Disc>();

        Vector2 centerVector = AngleCenterVector;
        Vector3 angleVector = (Vector3)centerVector * RegionTemplate.RadiusBase;

        _base.position = transform.position
                         + transform.TransformVector(
                             angleVector);
        _base.up = transform.TransformVector(centerVector);
        _regionCollider = GetComponent<PolygonCollider2D>();
        return _base;
    }

    [Button]
    public Transform TryCreateBase()
    {
        if (_base == null)
        {
            _base = new GameObject(defaultBaseName).transform;
            _base.SetParent(transform, false);
            InitBase();
        }

        return _base;
    }

    public Transform ResetBase()
    {
        if (_base == null) return TryCreateBase();
        return InitBase();
    }

    public void FindBase()
    {
        if (_base == null) _base = transform.Find(defaultBaseName);
    }

    #endregion

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
            if (targetIsWater)
                // Land to Water
                updateAction += value =>
                {
                    Thickness = Mathf.Lerp(RegionTemplate.Thickness, 0f, value);
                    RegionCollider.offset = Vector2.down * (RegionTemplate.Thickness - Thickness);
                };
            else
                // Water to Land
                updateAction += value =>
                {
                    Thickness = Mathf.Lerp(0f, RegionTemplate.Thickness, value);
                    RegionCollider.offset = Vector2.down * (RegionTemplate.Thickness - RegionDisc.Thickness);
                };
        }

        // Set Color
        updateAction += value =>
        {
            RegionDisc.Color = Color.Lerp(
                RegionsMan.configs[regionID].RegionColor,
                RegionsMan.configs[targetID].RegionColor,
                value);
        };

        GameObject goalPrefab = RegionsMan.configs[targetID].SurfacePrefab;
        bool goalBlankTag = goalPrefab == null || goalPrefab.CompareTag("Untagged");

        // Destroy inappropriate surface objects
        for (int index = 0; index < surfaceObjects.Count; index++)
        {
            Growable surfaceObject = surfaceObjects[index];
            if (goalPrefab == null)
                surfaceObjects[index].Destroy();
            // if goal is untagged or objects do not match goal objects
            else if ((goalBlankTag || !surfaceObject.gameObject.CompareTag(goalPrefab.tag)) &&
                !surfaceObject.isDestroyed)
                surfaceObjects[index].Destroy();
        }
        // Trim list of dying objects
        surfaceObjects = surfaceObjects
            .Where(growObj => growObj != null && !growObj.isDestroyed)
            .ToList();

        // Spawn new surface obj
        if (goalPrefab != null)
        {
            GameObject newSurfaceObj = Instantiate(goalPrefab, Base, false);
            Growable newGrowable = newSurfaceObj.GetComponentInChildren<Growable>(true);
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
        foreach (Growable surfaceObject in surfaceObjects)
        {
            surfaceObject.TryGrowState();
        }
    }

    [Button]
    public void Reset()
    {
        foreach (Growable surfaceObject in surfaceObjects)
        {
            surfaceObject.ResetState();
        }
    }

    #region Spatial Helpers

    /// <param name="_x">Percentage of region</param>
    /// <param name="_y">Percentage</param>
    /// <returns>World position</returns>
    public Vector2 RegionPosition(float _x, float _y = 1f)
    {
        return transform.position + transform.TransformVector(
            (Vector3)MathU.DegreeToVector2(
                MathU.LerpAngleUnclamped(AngleStart, AngleEnd, _x))
            * (RadiusBase + Mathf.LerpUnclamped(0f, Thickness, _y))
        );
    }

    public Vector2 RegionPosition(Vector2 _regionPosition)
    {
        return RegionPosition(_regionPosition.x, _regionPosition.y);
    }

    /// <returns>Returns Regions array index + 0..1</returns>
    public float WorldToRegionDistance(Vector2 _worldPos)
    {
        Vector2 localPos = transform.InverseTransformPoint(_worldPos);
        var angle = MathU.Vector2ToDegree(localPos.normalized);
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
        var angle = MathU.Vector2ToDegree(localPos.normalized);
        var segments = angle * (1f / Mathf.DeltaAngle(AngleStart, AngleEnd));

        var totalDst = Vector2.Distance(transform.position, localPos);
        var height = (totalDst - RadiusBase) * (1f / Thickness);

        return new Vector2(segments, height);
    }

    #endregion

    //private void TestUpdateBasePos()
    //{
    //    if (Base)
    //        Base.position = RegionPosition(defaultBasePosition);
    //}

    ////[Button]
    //public void MakeForest()
    //{
    //    for (var i = 0; i < Base.childCount; i++)
    //    {
    //        var child = Base.GetChild(i);
    //        Destroy(child.gameObject);
    //    }

    //    //RegionDisc.Color = forestColor;

    //    //Instantiate(forestPrefab, Base, false);
    //}
}