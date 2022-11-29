using MathUtility;
using NaughtyAttributes;
using Shapes;
using System;
using UnityEngine;
using UnityEngine.Events;
using static UnityUtility.Tween;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Region : MonoBehaviour
{
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

    public Wheel Wheel = null;

    public AnimCurve animTerraform = new();

    public RegionID currentID = RegionID.Water;
    public RegionID transformID = RegionID.Water;

    private Transform _base = null;
    private Disc _regionDisc = null;

    private readonly string defaultBaseName = "Base";
    private readonly Vector2 defaultBasePosition = new(0.5f, 0f);


    #region Properties

    public Disc RegionDisc
    {
        get
        {
            if (!_regionDisc) _regionDisc = GetComponent<Disc>();
            return _regionDisc;
        }
        set => _regionDisc = value;
    }

    public Transform Base => TryCreateBase();

    public float Thickness
    {
        get
        {
            if (RegionDisc.ThicknessSpace != ThicknessSpace.Meters)
                RegionDisc.ThicknessSpace = ThicknessSpace.Meters;
            return RegionDisc.Thickness;
        }
    }

    public float ThicknessHalf => Thickness * 0.5f;

    public float RadiusBase
    {
        get
        {
            if (RegionDisc.RadiusSpace != ThicknessSpace.Meters)
                RegionDisc.RadiusSpace = ThicknessSpace.Meters;
            return RegionDisc.Radius - ThicknessHalf;
        }
    }

    public float RadiusFull => RadiusBase + Thickness;

    public float AngleCenter => Mathf.LerpAngle(AngleStart, AngleEnd, 0.5f);

    public float AngleStart => RegionDisc.AngRadiansStart * Mathf.Rad2Deg;

    public float AngleEnd => RegionDisc.AngRadiansEnd * Mathf.Rad2Deg;

    public float AngleSize => Mathf.DeltaAngle(AngleStart, AngleEnd);

    public Vector2 AngleCenterVector => MathU.DegreeToVector2(AngleCenter);

    #endregion


    public void TerraformRegion(RegionID targetID)
    {
        transformID = targetID;
        if (transformID == currentID || animTerraform.Animating) return;
        currentID = transformID;

        animTerraform.Reset();
        animTerraform.Play();

        UnityAction<float> updateAction;
        updateAction = targetID == RegionID.Water
            ? delegate(float value)
            {
                if (RegionDisc.ThicknessSpace != ThicknessSpace.Meters)
                    RegionDisc.ThicknessSpace = ThicknessSpace.Meters;
                RegionDisc.Thickness = Mathf.Lerp(Wheel.regions.regionTemplate.Thickness, 0, value);
                Debug.Log(RegionDisc.Thickness);
            }
            : delegate(float value)
            {
                if (RegionDisc.ThicknessSpace != ThicknessSpace.Meters)
                    RegionDisc.ThicknessSpace = ThicknessSpace.Meters;
                RegionDisc.Thickness = Mathf.Lerp(0, Wheel.regions.regionTemplate.Thickness, value);
                Debug.Log(RegionDisc.Thickness);
            };

        animTerraform.Updated.AddListener(updateAction);

        UnityAction<float> finishAction =
            delegate { animTerraform.Updated.RemoveListener(updateAction); };
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

    private void Update()
    {
        animTerraform.Update();
    }

    private void Awake()
    {
        RegionDisc ??= GetComponent<Disc>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(RegionPosition(defaultBasePosition), 0.1f);
    }

    private Transform InitBase()
    {
        RegionDisc ??= GetComponent<Disc>();
        _base.position = transform.position
                         + transform.TransformVector(
                             (Vector3) AngleCenterVector * RadiusBase);
        _base.up = transform.TransformVector(AngleCenterVector);
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
        if (_base == null) TryCreateBase();
        return InitBase();
    }

    #region Spatial Helpers

    /// <param name="_x">Percentage of region</param>
    /// <param name="_y">Percentage</param>
    /// <returns>World position</returns>
    public Vector2 RegionPosition(float _x, float _y = 1f)
    {
        return transform.position + transform.TransformVector(
            (Vector3) MathU.DegreeToVector2(
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
        return (int) Mathf.Floor(repeat);
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

    private void TestUpdateBasePos()
    {
        if (Base)
            Base.position = RegionPosition(defaultBasePosition);
    }


    [Button()]
    public void MakeForest()
    {
        for (var i = 0; i < Base.childCount; i++)
        {
            var child = Base.GetChild(i);
            Destroy(child.gameObject);
        }

        //RegionDisc.Color = forestColor;

        //Instantiate(forestPrefab, Base, false);
    }

    //0 = nothing on tile
    [Button()]
    public void Tick()
    {
        var ani = gameObject.GetComponentInChildren<Animator>();
        var progress = ani.GetInteger("Progress");
        var max = ani.GetInteger("MaxProgress");
        ani.SetInteger("Progress", Math.Min(progress + 1, max));
    }

    [Button()]
    public void ResetTick()
    {
        var ani = gameObject.GetComponentInChildren<Animator>();
        ani.SetInteger("Progress", 0);
    }

    public void FindBase()
    {
        if (_base == null) _base = transform.Find(defaultBaseName);
    }
}