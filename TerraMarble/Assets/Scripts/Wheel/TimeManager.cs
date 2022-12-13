using System.Collections;
using System.Collections.Generic;
using FunkyCode;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeManager : MonoBehaviour
{
    [Header("Config")] [CurveRange(0f, 0f, 1f, 1f)]
    public AnimationCurve dayCurve;

    [Range(0f, 1f)] [OnValueChanged("UpdateSunTimeEffects")]
    public float sunTimeCurrent = 0f;

    public float timeSpeed = 0.05f;

    [CurveRange(0f, 0f, 1f, 1f)] public AnimationCurve cameraEffectCurve;

    public bool advancingDay = true;

    [Header("Data")] [Range(0f, 2f)] public float realDayTime = 0f;

    [Header("Night Effects")] [SerializeField]
    private Light2D[] lights;

    [SerializeField] private ParticleSystem starsParticles = null;
    [Header("Debug")] [SerializeField] private LightCycle lightCycle = null;
    [SerializeField] private Volume nightVolume = null;

    private Material starsMaterial = null;

    private void Awake()
    {
        lightCycle ??= FindObjectOfType<LightCycle>();
        nightVolume ??= Camera.main.gameObject.GetComponent<Volume>();
        starsMaterial ??= starsParticles?.GetComponent<ParticleSystemRenderer>()?.material;
    }

    private void Update()
    {
        if (!advancingDay) return;

        float delta = Time.deltaTime * timeSpeed;
        realDayTime = Mathf.Repeat(realDayTime + delta, 2f);
        sunTimeCurrent = dayCurve.Evaluate(Mathf.PingPong(realDayTime, 1f));

        UpdateSunTimeEffects();
    }

    private void UpdateSunTimeEffects()
    {
        lightCycle ??= FindObjectOfType<LightCycle>();
        nightVolume ??= Camera.main.gameObject.GetComponent<Volume>();

        if (lightCycle) lightCycle.time = sunTimeCurrent;
        float nightWeight = cameraEffectCurve.Evaluate(sunTimeCurrent);
        if (nightVolume) nightVolume.weight = nightWeight;

        foreach (Light2D light2D in lights)
            if (light2D)
                light2D.color.a = nightWeight;

        if (starsParticles)
        {
            if (!starsMaterial)
                starsMaterial ??= starsParticles?.GetComponent<ParticleSystemRenderer>()?.material;
            if (starsMaterial)
            {
                Color starsMaterialTint = starsMaterial.color;
                starsMaterialTint.a = nightWeight;
                starsMaterial.color = starsMaterialTint;
            }

            if (sunTimeCurrent <= 0f && !starsParticles.isPaused)
                starsParticles.Pause();
            else if (sunTimeCurrent > 0f && starsParticles.isPaused)
                starsParticles.Play();
        }
    }

    private void OnApplicationQuit()
    {
        Color starsMaterialTint = starsMaterial.color;
        starsMaterialTint.a = 1f;
        starsMaterial.color = starsMaterialTint;
    }
}