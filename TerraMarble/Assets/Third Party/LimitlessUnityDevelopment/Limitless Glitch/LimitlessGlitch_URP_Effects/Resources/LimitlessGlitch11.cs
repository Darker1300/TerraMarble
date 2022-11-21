using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Limitless Glitch/Glitch11")]
public class LimitlessGlitch11 : VolumeComponent, IPostProcessComponent
{
	public BoolParameter enable = new BoolParameter(false);
	[Tooltip("Effect amount")]
	public ClampedFloatParameter amount = new ClampedFloatParameter(0.0021f, 0, .02f);
	[Tooltip("Floating Lines Amount")]
	public ClampedFloatParameter linesAmount = new ClampedFloatParameter(1f, 0, 10);
	[Tooltip("Lines speed")]
	public ClampedFloatParameter speed = new ClampedFloatParameter(1f, 0, 10);
	[Space]
	[Tooltip("Mask texture")]
	public TextureParameter mask = new TextureParameter(null);

	public bool IsActive() => (bool)enable;

	public bool IsTileCompatible() => false;
}