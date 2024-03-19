using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathUtility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Pool;
using UnityUtility;

public class CloudsManager : MonoBehaviour
{
    [SerializeField] private Wheel wheel;
    [SerializeField] private GameObject cloudPrefab = null;
    [SerializeField] private Transform cloudsParent = null;

    [Header("Config")]

    [SerializeField] private float cloudSpawnTime = 0.5f;
    [SerializeField] private int cloudMaxCount = 50;

    [SerializeField] private bool preloadClouds = true;
    [SerializeField] private bool updateClouds = false;

    [SerializeField] private Vector2 lifeTimeRange = new(5f, 15f);

    [SerializeField] private Vector2 heightRange = new(28, 50);

    [SerializeField] private Vector2 scaleRange = new(5, 7);

    [SerializeField] private Vector2 speedRange = new(1.5f, .75f);

    [SerializeField]
    private AnimationCurve opacityOverLifetimeCurve
        = new(new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f));

    [SerializeField] private bool doDebug = false;

    [Header("Data")] [SerializeField] private float cloudSpawnTimer = 0f;
    [SerializeField] private ObjectPool<CloudData> cloudPool = null;
    [SerializeField] private List<CloudData> activeClouds = null;
    [SerializeField] private List<CloudData> tempDestroyClouds = null;

    public class ShaderParam
    {
        public static int Opacity = Shader.PropertyToID("_Opacity");
    }

    private void Start()
    {
        activeClouds ??= new();
        tempDestroyClouds ??= new();

        cloudsParent ??= transform;
        cloudPool = new ObjectPool<CloudData>(
            () => Instantiate(cloudPrefab, cloudsParent, false).GetComponent<CloudData>(),
            OnGet,
            OnRelease,
            data => Destroy(data.gameObject),
            true, 10,
            200
        );

        if (preloadClouds)
        {
            //Vector2 wheelPosition = wheel.transform.position.To2DXY();
            for (int i = 0; i < cloudMaxCount; i++)
            {
                var cloud = SpawnCloud();
                cloud.timeRemaining = cloud.lifeTime * 0.5f;
                UpdateCloudMaterial(cloud);
            }
        }
    }

    private void OnGet(CloudData cloud)
    {
        activeClouds.Add(cloud);
        cloud.gameObject.SetActive(true);
    }

    private void OnRelease(CloudData cloud)
    {
        activeClouds.Remove(cloud);
        cloud.gameObject.SetActive(false);
    }

    private void Update()
    {
        Vector2 wheelPosition = wheel.transform.position.To2DXY();
        float deltaTime = Time.deltaTime;

        foreach (var cloud in activeClouds)
        {
            if (updateClouds)
                cloud.timeRemaining -= deltaTime;
            if (cloud.timeRemaining > 0f)
            {
                UpdateCloud(cloud, wheelPosition, deltaTime);
                if (updateClouds)
                    UpdateCloudMaterial(cloud);
            }
            else // else destroy
                tempDestroyClouds.Add(cloud);
        }

        if (!updateClouds) return;

        // destroy clouds
        foreach (var cloud in tempDestroyClouds)
            cloudPool.Release(cloud);
        tempDestroyClouds.Clear();

        cloudSpawnTimer -= deltaTime;

        // safety guard against infinite loop;
        if (cloudSpawnTime < float.Epsilon) return;

        while (cloudSpawnTimer < float.Epsilon)
        {
            cloudSpawnTimer += cloudSpawnTime;
        }
    }

    [Button]
    private CloudData SpawnCloud()
    {
        CloudData cloud = cloudPool.Get();

        float cloudLifeTime = Random.Range(lifeTimeRange.x, lifeTimeRange.y);
        cloud.lifeTime = cloudLifeTime;
        cloud.timeRemaining = cloudLifeTime;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(heightRange.x, heightRange.y);
        Vector2 localPoint = randomDirection * randomDistance;

        float scaleRandom = Random.Range(0f, 1f);
        cloud.speed = Mathf.Lerp(speedRange.x, speedRange.y, scaleRandom);

        float newScale = Mathf.Lerp(scaleRange.x, scaleRange.y, scaleRandom);
        cloud.scale = newScale;
        cloud.transform.localScale = newScale * Vector3.one;

        Vector3 newPos = localPoint.To3DXY(-.1f - scaleRandom);
        cloud.transform.localPosition = newPos;
        cloud.height = randomDistance;

        cloud.transform.up = ((Vector2)wheel.transform.position)
            .Towards(newPos).normalized.To3DXY(0f);

        if (cloud.meshRenderer == null)
            cloud.meshRenderer = cloud.GetComponent<MeshRenderer>();
        UpdateCloudMaterial(cloud);
        return cloud;
    }

    private void UpdateCloud(CloudData cloud, Vector2 wheelPosition, float deltaTime)
    {
        Vector3 originalPos = cloud.transform.position;
        cloud.transform.position =
            originalPos
                .To2DXY()
                .RotateAround(cloud.speed * deltaTime, wheelPosition)
                .To3DXY(originalPos.z);

        
        cloud.transform.up = wheel.transform.position.To2DXY()
            .Towards(cloud.transform.position)
            .normalized.To3DXY(0f);
    }

    private void UpdateCloudMaterial(CloudData cloud)
    {
        float opacity = opacityOverLifetimeCurve
            .Evaluate(1f - cloud.timeRemaining * (1f / cloud.lifeTime));
        cloud.meshRenderer.material
            .SetFloat(ShaderParam.Opacity, opacity);
    }


    private void OnDrawGizmosSelected()
    {
        if (!doDebug) return;
        if (wheel == null) wheel = FindObjectOfType<Wheel>();

        Gizmos.color = Color.white;
        GizmosExtensions.DrawWireCircle(wheel.transform.position, heightRange.x,
            72, Quaternion.LookRotation(Vector3.up, Vector3.forward));
        GizmosExtensions.DrawWireCircle(wheel.transform.position, heightRange.y,
            72, Quaternion.LookRotation(Vector3.up, Vector3.forward));
    }
}