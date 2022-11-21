using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[VolumeComponentMenu("Limitless Glitch/Glitch8")]
public class LimitlessGlitch8 : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);

    [Range(1f, 0f), Tooltip("Effect amount.")]
    public ClampedFloatParameter Amount = new ClampedFloatParameter(0.5f,0f, 1f);
    [Range(0.1f, 20f), Tooltip("Glitch lines width.")]
    public ClampedFloatParameter LinesWidth = new ClampedFloatParameter(1f, 0.1f, 20f);
    [Range(0f, 1f), Tooltip("Effect speed.")]
    public ClampedFloatParameter Speed = new ClampedFloatParameter (1f,0f, 1f);
    [Range(0f, 13f), Tooltip("Offset on X axis.")]
    public ClampedFloatParameter Offset = new ClampedFloatParameter(1f,0f, 13f);
    [Range(0f, 1f), Tooltip("Effect alpha.")]
    public ClampedFloatParameter alpha = new ClampedFloatParameter(1f,0f, 1f);

    [Space]
    [Tooltip("Time.unscaledTime .")]
    public BoolParameter unscaledTime = new BoolParameter (false);
    [Space]
    [Tooltip("Mask texture")]
    public TextureParameter mask = new TextureParameter(null);

    public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}