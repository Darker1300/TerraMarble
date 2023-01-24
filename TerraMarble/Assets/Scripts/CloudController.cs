using System;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using MathUtility;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Scripting;
using UnityUtility;
using static UnityEngine.ParticleSystem;

public class CloudController : MonoBehaviour
{
    private new ParticleSystem particleSystem = null;
    private Wheel wheel = null;
    [SerializeField] private GameObject cloudPrefab = null;
    [SerializeField] private Transform cloudsParent = null;

    private Particle[] currentParticles = null;
    private Dictionary<Particle, GameObject> prevParticles = null;
    private ObjectPool<GameObject> cloudPool = null;

    // [SerializeField] private Material cloudMatPrefab = null;
    // private Material cloudMat = null;
    [SerializeField]
    private AnimationCurve curve
        = new(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f));

    public class ShaderParam
    {
        public static int Opacity = Shader.PropertyToID("_Opacity");
    }

    private void Awake()
    {
        particleSystem ??= GetComponentInChildren<ParticleSystem>();
        wheel ??= FindObjectOfType<Wheel>();

        cloudsParent ??= transform;
        cloudPool = new ObjectPool<GameObject>(
            () => Instantiate(cloudPrefab, cloudsParent, false),
            cloud => cloud.SetActive(true),
            cloud => cloud.SetActive(false),
            cloud => Destroy(cloud),
            true, 10
        );
        prevParticles = new Dictionary<Particle, GameObject>();
        // cloudMat = Instantiate(cloudMatPrefab);
    }

    private void OnEnable()
    {
        //if (!cloudMatPrefab) return;
        // cloudMat = Instantiate(cloudMatPrefab);
        //foreach (var pair in particleCloudPairs)
        //     pair.Value.GetComponent<MeshRenderer>().sharedMaterial = cloudMat;
    }

    private void OnDisable()
    {
        //if (!cloudMatPrefab || !cloudMat) return;
        // Destroy(cloudMat);
    }


    private void LateUpdate()
    {
        UpdateClouds();
    }


    [Button]
    public void ResetPool()
    {
        currentParticles = null;
        prevParticles.Clear();
        cloudPool.Clear();
    }

    [Button]
    public void UpdateClouds()
    {
        //if (currentParticles == null || currentParticles.Length != particleSystem.particleCount)
        //currentParticles = new Particle[particleSystem.particleCount];
        if (currentParticles == null || particleSystem.particleCount > currentParticles.Length)
            currentParticles = new Particle[particleSystem.particleCount];
        int currentAliveCount = particleSystem.GetParticles(currentParticles);

        //// Untested:

        //List<(Particle part, bool doDestroy)> partJobs = new List<(Particle part, bool doDestroy)>();

        //bool prevIsBigger = prevParticles.Keys.Count >= currentParticles.Length;
        //Particle[] partsBigger = prevIsBigger ? (prevParticles.Keys).ToArray() : currentParticles;
        //Particle[] partsSmaller = !prevIsBigger ? (prevParticles.Keys).ToArray() : currentParticles;
        //int iMax = Mathf.Max(prevParticles.Keys.Count, currentParticles.Length);

        //for (int i = 0, sI = 0, bI = 0;
        //    i < iMax;
        //    i++, sI++, bI++)
        //{
        //    //// Destroy ?
        //    if (sI < partsSmaller.Length)
        //    {
        //        Particle sPart = partsSmaller.ElementAt(sI);
        //        bool smallIsInBig = partsBigger.Contains(sPart);
        //        partJobs.Add((sPart, prevIsBigger));
        //    }

        //    // Create ?
        //    if (bI < partsBigger.Length)
        //    {
        //        Particle bPart = partsBigger.ElementAt(bI);
        //        bool bigIsInSmall = partsSmaller.Contains(bPart);
        //        partJobs.Add((bPart, !prevIsBigger));
        //    }
        //}

        //// Perform
        //for (int i = 0; i < partJobs.Count; i++)
        //{
        //    var job = partJobs[i];
        //    if (!job.doDestroy)
        //        CreateCloud(job.part);
        //    else
        //        DestroyCloud(job.part);
        //}

        //// Destroy
        for (int i = 0; i < prevParticles.Keys.Count; i++)
        {
            Particle existPart = prevParticles.Keys.ElementAt(i);
            DestroyCloud(existPart);
        }

        //// Create
        for (int i = 0; i < currentParticles.Length; i++)
        {
            Particle newPart = currentParticles.ElementAt(i);
            bool isNew = !prevParticles.Keys.Contains(newPart);

            if (isNew)
                CreateCloud(newPart);
        }

        // Update
        foreach (var particlePair in prevParticles)
        {
            bool exists = currentParticles.Contains(particlePair.Key);
            if (!exists) continue;

            //bool exists = prevParticles.TryGetValue(particle, out GameObject cloudGO);
            //if (!exists) continue;
            UpdateCloud(particlePair);
        }

        // particleSystem.SetParticles(currentParticles);

        #region OLD

        //// var existingParticlesHash = new HashSet<uint>(particleCloudPairs.Keys.Select(p => p.randomSeed));
        //// var newParticles = currentParticles.Where(p => !existingParticlesHash.Contains(p.randomSeed)).ToArray();
        //// var newdParticles = currentParticles.Where(p => !newParticles.Contains(p.randomSeed)).ToArray();

        //// Create
        //foreach (var particle in newParticles)
        //    CreateCloud(particle);

        //// Destroy


        ////int id = Shader.PropertyToID("Opacity");
        //foreach (var (particle, cloudGO) in particleCloudPairs)
        //{
        //    float size = particle.startSize;
        //    float factor = Mathf.InverseLerp(0f, particle.startLifetime, particle.remainingLifetime);
        //    cloudGO.transform.localScale = new Vector3(size, size * factor, size);

        //    //float alpha = pair.Key.GetCurrentColor(particleSystem).a / 255f;
        //    //pair.Value.GetComponent<MeshRenderer>().material.SetFloat(id, alpha)
        //}

        ////particleSystem.SetParticles(currentParticles, currentAliveCount);


        ////if (changeCount == 0) return;

        //// Check for dead
        //foreach (var particle in particleCloudPairs.Keys)
        //    if (particle.remainingLifetime <= 0f)
        //    {

        //    }

        ////  // Died
        ////  particleCloudPairs.Keys.Except(currentParticles).ToList()
        ////      .ForEach(particle =>
        ////      {
        ////          particleCloudPairs.Remove(particle, out GameObject cloudGo);
        ////          cloudPool.Release(cloudGo);
        ////      });

        //// Created
        //if (cloudPrefab != null)
        //{
        //    //Particle[] newParts = currentParticles.Except(particleCloudPairs.Keys).ToArray();

        //    //var excludedIDs = new HashSet<uint>(particleCloudPairs.Keys.Select(p => p.randomSeed));
        //    //var newParts = currentParticles.Where(p => !excludedIDs.Contains(p.randomSeed)).ToArray();

        //    //foreach (var particle in newParts)
        //    //{
        //    //    GameObject cloudGo = cloudPool.Get();
        //    //    particleCloudPairs.Add(particle, cloudGo);

        //    //    Transform cT = cloudGo.transform;
        //    //    bool isWorld = particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World;
        //    //    cT.position = isWorld 
        //    //        ? particle.position 
        //    //        : particleSystem.transform.TransformPoint(particle.position);
        //    //    cT.up = ((Vector2)wheel.transform.position)
        //    //        .Towards((Vector2)cT.position)
        //    //        .normalized;

        //    //    cT.localScale = particle.startSize * Vector3.one;
        //    //}
        //}

        #endregion
    }

    private void CreateCloud(Particle particle)
    {
        GameObject cloudGo = cloudPool.Get();
        prevParticles.Add(particle, cloudGo);

        // Position
        Transform cT = cloudGo.transform;
        bool isWorld = particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World;
        cT.position = isWorld
            ? particle.position
            : particleSystem.transform.TransformPoint(particle.position);
        // sort
        cT.position += Vector3.forward * Mathf.InverseLerp(
            particleSystem.main.startSize.constantMax,
            particleSystem.main.startSize.constantMin,
            particle.startSize);
        // Rotation
        cT.up = ((Vector2)wheel.transform.position)
            .Towards((Vector2)cT.position)
            .normalized;
        // Scale
        cT.localScale = particle.startSize * Vector3.one;

        //cloudGO.GetComponent<MeshRenderer>().material.SetFloat(ShaderParam.Opacity, factor);
    }

    private void UpdateCloud(KeyValuePair<Particle, GameObject> partGOPair)
    {
        float size = partGOPair.Key.startSize;
        float factor = Mathf.InverseLerp(partGOPair.Key.startLifetime, 0f, partGOPair.Key.remainingLifetime);
        factor = curve.Evaluate(factor);

        partGOPair.Value.transform.localScale = new Vector3(size, size * factor, size);

        partGOPair.Value
            .GetComponent<MeshRenderer>()
            .material
            .SetFloat(ShaderParam.Opacity, factor);
    }

    private void DestroyCloud(Particle particle)
    {
        bool exists = prevParticles.Remove(particle, out GameObject cloudGo);
        if (!exists) return;

        cloudPool.Release(cloudGo);
    }
}