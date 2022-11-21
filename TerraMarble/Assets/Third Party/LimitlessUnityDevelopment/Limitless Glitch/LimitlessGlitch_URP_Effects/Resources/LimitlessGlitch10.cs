using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Limitless Glitch/Glitch10")]
public class LimitlessGlitch10 : VolumeComponent, IPostProcessComponent
{
	public BoolParameter enable = new BoolParameter(false);
	[Tooltip("Outline width")]
	public ClampedFloatParameter width = new ClampedFloatParameter(0.3f, 0, .5f);
	[Tooltip("Effect fade")]
	public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0, 1);
	[Space]
	[Tooltip("Mask texture")]
	public TextureParameter mask = new TextureParameter(null);
	public bool IsActive() => (bool)enable;

	public bool IsTileCompatible() => false;
}