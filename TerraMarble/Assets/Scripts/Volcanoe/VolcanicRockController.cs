using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;

public class VolcanicRockController : MonoBehaviour
{
    private WheelRegionsManager regionsMan;
    private new Collider2D collider2D;
    private new Rigidbody2D rigidbody2D;
    private PoolObject poolObject;
    [SerializeField] private GameObject SmokePrefab = null;
    [SerializeField] private bool IsAlive = false;
    [SerializeField] private bool isMovingDownward = false;

    [SerializeField] private List<Collider2D> ignoredColliders = new List<Collider2D>();

    private Vector2 lastFrameVelocity = Vector2.zero;

    void Start()
    {
        regionsMan ??= FindObjectOfType<WheelRegionsManager>();
        poolObject ??= GetComponent<PoolObject>();
        collider2D = GetComponent<Collider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        IsAlive = true;
        foreach (Collider2D col in ignoredColliders)
            Physics2D.IgnoreCollision(collider2D, col, false);
        ignoredColliders.Clear();
    }

    private void OnDisable()
    {
        IsAlive = false;
    }

    private void FixedUpdate()
    {
        if (IsAlive)
            lastFrameVelocity = rigidbody2D.velocity;
    }

    private void DoDestroy(Region.RegionHitInfo info)
    {
        if (!IsAlive) return;
        IsAlive = false;

        if (!SmokePrefab) return;

        // Particle fx
        GameObject go = Instantiate(SmokePrefab, info.region.Base, false);
        go.transform.position = info.collision.GetContact(0).point;
        go.transform.localRotation = SmokePrefab.transform.localRotation;

        if (poolObject is not null)
            poolObject.Pool.ReturnToPool(gameObject);
        else Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsAlive) return;

        Region.RegionHitInfo info = regionsMan.ProcessWheelHit(collision, null);

        if (info is not null)
        {
            bool doDestroy = false;

            if (info.region.regionID == Region.RegionID.Water)
            {
                // do create Land
                info.region.TerraformRegion(Region.RegionID.Rock);
                doDestroy = true;
                info.region.StartCoroutine(
                    (null, o => new WaitForSeconds(0.15f)),
                    (info as object, o =>
                        {
                            var rInfo = o as Region.RegionHitInfo;
                            rInfo.region.GetAdjacentRegion(1)
                                .TerraformRegion(Region.RegionID.Rock);
                            rInfo.region.GetAdjacentRegion(-1)
                                .TerraformRegion(Region.RegionID.Rock);
                            return null;
                        }
                )
                );
            }
            else
            {
                bool hitObject = info.surfaceObj is not null;

                if (isMovingDownward || !hitObject)
                {
                    // hit a Land region, destroy land
                    info.region.TerraformRegion(Region.RegionID.Water);
                    doDestroy = true;
                    info.region.StartCoroutine(
                            (null, o => new WaitForSeconds(0.15f)),
                            (info as object, o =>
                            {
                                var rInfo = o as Region.RegionHitInfo;
                                rInfo.region.GetAdjacentRegion(1)
                                    .TerraformRegion(Region.RegionID.Water);
                                rInfo.region.GetAdjacentRegion(-1)
                                    .TerraformRegion(Region.RegionID.Water);
                                return null;
                            }
                    )
                        );
                }
                else if (hitObject)
                {
                    // hit tree or other surface object
                    info.surfaceObj.DoDestroy();

                    // ignore collision and keep going
                    ignoredColliders.Add(info.collision.collider);
                    Physics2D.IgnoreCollision(collider2D, info.collision.collider, true);
                    info.collision.otherRigidbody.velocity = lastFrameVelocity;
                }
            }

            if (doDestroy) DoDestroy(info);
        }
    }
}
