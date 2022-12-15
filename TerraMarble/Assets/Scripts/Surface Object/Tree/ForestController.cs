using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class ForestController : MonoBehaviour
{
    private SurfaceObject surfaceObject = null;
    private Region region = null;
    private Growable growable = null;

    #region Events

    void Awake()
    {
        surfaceObject = GetComponent<SurfaceObject>();
        region = GetComponentInParent<Region>();
        growable = GetComponent<Growable>();

        surfaceObject.DestroyStart.AddListener(OnDestroyStart);
        region.BallHitEnter.AddListener(OnBallHitEnter);
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        if (surfaceObject.isDestroyed
            & growable.IsInEmptyState())
        {
            surfaceObject.DestroyEnd.Invoke();
            region.surfaceObjects.Remove(surfaceObject);
            Destroy(gameObject);
        }
    }

    void OnBallHitEnter(Region.RegionHitInfo info)
    {
        if (info.ballState is not null && info.ballState.Stomp)
            OnBallStompEnter(info);
    }

    void OnBallStompEnter(Region.RegionHitInfo info)
    {
        if (info.surfaceObj != null)
        {   // hit tree
            ShrinkDestroy();
        }
    }

    #endregion

    public void TryGrow()
    {
        if (surfaceObject.isDestroyed) return;
        growable.TryGrowState();
    }

    public void TryShrink()
    {
        if (surfaceObject.isDestroyed) return;
        growable.TryShrinkState();
    }

    public void Reset()
    {
        if (surfaceObject.isDestroyed) return;
        growable.ResetState();
    }

    public void ShrinkDestroy()
    {
        bool success = growable.TryShrinkState();
        if (!success && !surfaceObject.isDestroyed)
            surfaceObject.DoDestroy();
    }

    void OnDestroyStart()
    {
        growable.ResetState();
    }
}
