using UnityEngine;
using UnityEngine.Events;
using UnityUtility;

public class VolcanicRockController : MonoBehaviour
{
    private WheelRegionsManager regionsMan;
    private new Rigidbody2D rigidbody2D;
    [SerializeField] private GameObject SmokePrefab = null;
    [SerializeField] private bool IsAlive = false;
    [SerializeField] private bool isMovingDownward = false;

    void Start()
    {
        regionsMan ??= FindObjectOfType<WheelRegionsManager>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        IsAlive = true;
    }

    private void OnDisable()
    {
        IsAlive = false;
    }

    private void DoDestroy(Region.RegionHitInfo info)
    {
        if (!IsAlive) return;
        IsAlive = false;

        if (!SmokePrefab) return;
        Vector3 p = info.region.Base.InverseTransformPoint(info.collision.GetContact(0).point);
        GameObject go = Instantiate(SmokePrefab,
            p,
            Quaternion.identity,
            info.region.Base);
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

                this.StartCoroutine(
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

                    this.StartCoroutine(
                            (null, o => new WaitForSeconds(0.15f)),
                            (info as object, o =>
                            {
                                var rInfo = o as Region.RegionHitInfo;
                                rInfo.region.GetAdjacentRegion(1)
                                    .TerraformRegion(Region.RegionID.Water);
                                rInfo.region.GetAdjacentRegion(-1)
                                    .TerraformRegion(Region.RegionID.Water); 
                                return null;
                            })
                        );

                }
                else if (hitObject)
                {
                    // hit tree or other surface object
                    info.surfaceObj.DoDestroy();
                }
            }

            if (doDestroy) DoDestroy(info);
        }
    }
}
