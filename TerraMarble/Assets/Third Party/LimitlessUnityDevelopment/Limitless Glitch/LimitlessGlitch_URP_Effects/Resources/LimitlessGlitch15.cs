using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[VolumeComponentMenu("Limitless Glitch/Glitch15")]

public class LimitlessGlitch15 : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
    [Tooltip("Dropout Intensity.")]
    public ClampedFloatParameter dropoutIntensity = new ClampedFloatParameter(0.3f, 0, 2f);
    [Tooltip("Interlace Intesnsity.")]
    public ClampedFloatParameter interlaceIntesnsity = new ClampedFloatParameter(0.21f, 0, 2f);
    [Space]
    [Tooltip("Mask texture")]
    public TextureParameter mask = new TextureParameter(null);
    public bool IsActive() => (bool)enable;
    public bool IsTileCompatible() => false;
}