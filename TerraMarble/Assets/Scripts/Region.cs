using System;
using System.Numerics;
using MathUtility;
using NaughtyAttributes;
using Shapes;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Region : MonoBehaviour
{
    public Wheel Wheel = null;
    private Transform _base = null;
    private Disc _regionDisc = null;

    public Color forestColor;
    public GameObject forestPrefab;



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

    public float BaseRadius
    {
        get
        {
            if (RegionDisc.RadiusSpace != ThicknessSpace.Meters)
                RegionDisc.RadiusSpace = ThicknessSpace.Meters;
            return RegionDisc.Radius - ThicknessHalf;
        }
    }

    public float FullRadius => BaseRadius + Thickness;

    public float CenterAngle => Mathf.LerpAngle(StartAngle, EndAngle, 0.5f);

    public float StartAngle => RegionDisc.AngRadiansStart * Mathf.Rad2Deg;

    public float EndAngle => RegionDisc.AngRadiansEnd * Mathf.Rad2Deg;

    public Vector2 CenterAngleVector => MathU.DegreeToVector2(CenterAngle);


    [Header("Debug")]
    [SerializeField]
    [OnValueChanged("TestUpdateBasePos")]
    [Range(0f, 1f)]
    private float baseX = 0.5f;

    [SerializeField]
    [OnValueChanged("TestUpdateBasePos")]
    [Range(0f, 1f)]
    private float baseY = 0.0f;

    private void Awake()
    {
        RegionDisc ??= GetComponent<Disc>();
    }

    [Button]
    public Transform TryCreateBase()
    {
        if (_base == null)
        {
            RegionDisc ??= GetComponent<Disc>();
            _base = new GameObject("Base").transform;
            _base.SetParent(transform, false);
            _base.position = transform.position
                             + transform.TransformVector(
                                 (Vector3)CenterAngleVector * BaseRadius);
            _base.up = CenterAngleVector;
        }

        return _base;
    }

    /// <param name="_x">Percentage</param>
    /// <param name="_y">Percentage</param>
    /// <returns>World position</returns>
    public Vector2 RegionPosition(float _x, float _y)
    {
        return transform.position + transform.TransformVector(
                   (Vector3)MathU.DegreeToVector2(
                       MathU.LerpAngleUnclamped(StartAngle, EndAngle, _x))
                   * (BaseRadius + Mathf.LerpUnclamped(0f, Thickness, _y))
               );
    }
    /// <param name="_regX">Percentage</param>
    /// <param name="_regY">Percentage</param>
    /// <returns>World position</returns>
    public Vector2 WorldToRegion(Vector2 worldPos)
    {
        Vector2 localPos = transform.InverseTransformPoint(worldPos);
        float angle = MathU.Vector2ToDegree(localPos.normalized);
        float segments = angle * (1f / Mathf.DeltaAngle(StartAngle, EndAngle));

        float totalDst = Vector2.Distance(transform.position, localPos);
        float height = (totalDst - BaseRadius) * (1f / Thickness);

        return new Vector2(segments, height);
    }

    private void TestUpdateBasePos()
    {
        if (Base)
            Base.position = RegionPosition(baseX, baseY);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(RegionPosition(baseX, baseY), 0.1f);
    }

    [Button()]
    public void MakeForest()
    {
        for (int i = 0; i < Base.childCount; i++)
        {
            var child = Base.GetChild(i);
            Destroy(child);
        }

        RegionDisc.Color = forestColor;

        Instantiate(forestPrefab, Base, false);
    }

    [Button()]
    public void Tick()
    {
        var ani = gameObject.GetComponentInChildren<Animator>();
        int progress = ani.GetInteger("Progress");
        int max = ani.GetInteger("MaxProgress");
        ani.SetInteger("Progress", Math.Min(progress + 1, max));
    }

    [Button()]
    public void ResetTick()
    {
        var ani = gameObject.GetComponentInChildren<Animator>();
        ani.SetInteger("Progress", 0);
    }
}