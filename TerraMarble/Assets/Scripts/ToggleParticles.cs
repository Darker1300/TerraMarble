using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ToggleParticles : MonoBehaviour
{
    [InfoBox("Particle System should be set to Looping: true, and Play On Awake: false.")]
    public ParticleSystem ParticleSystem = null;

    private void Start()
    {
        ParticleSystem = ParticleSystem != null ? ParticleSystem
            : GetComponent<ParticleSystem>();
    }

    [Button]
    public void Toggle()
    {
        ParticleSystem.EmissionModule emission = ParticleSystem.emission;
        emission.enabled = !emission.enabled;
        if (emission.enabled && !ParticleSystem.isPlaying)
            ParticleSystem.Play();
    }

    [Button]
    public void Play()
    {
        ParticleSystem.EmissionModule emission = ParticleSystem.emission;
        emission.enabled = true;
        if (!ParticleSystem.isPlaying)
            ParticleSystem.Play();
    }

    [Button]
    public void Pause()
    {
        ParticleSystem.EmissionModule emission = ParticleSystem.emission;
        emission.enabled = false;
    }

    public bool IsPlaying() 
    {
        return ParticleSystem.emission.enabled && ParticleSystem.isPlaying;
    }
}
