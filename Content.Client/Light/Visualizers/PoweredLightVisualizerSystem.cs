using System;
using System.Runtime.CompilerServices;
using Content.Shared.Light;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Light.Visualizers
{
	// Token: 0x02000267 RID: 615
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class PoweredLightVisualizerSystem : VisualizerSystem<PoweredLightVisualsComponent>
	{
		// Token: 0x06000FB9 RID: 4025 RVA: 0x0005EC76 File Offset: 0x0005CE76
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PoweredLightVisualsComponent, AnimationCompletedEvent>(new ComponentEventHandler<PoweredLightVisualsComponent, AnimationCompletedEvent>(this.OnAnimationCompleted), null, null);
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x0005EC94 File Offset: 0x0005CE94
		protected override void OnAppearanceChange(EntityUid uid, PoweredLightVisualsComponent comp, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			PoweredLightState poweredLightState;
			if (!this.AppearanceSystem.TryGetData<PoweredLightState>(uid, PoweredLightVisuals.BulbState, ref poweredLightState, args.Component))
			{
				return;
			}
			string text;
			if (comp.SpriteStateMap.TryGetValue(poweredLightState, out text))
			{
				args.Sprite.LayerSetState(PoweredLightLayers.Base, text);
			}
			bool flag;
			this.SetBlinkingAnimation(uid, poweredLightState == PoweredLightState.On && (this.AppearanceSystem.TryGetData<bool>(uid, PoweredLightVisuals.Blinking, ref flag, args.Component) && flag), comp);
		}

		// Token: 0x06000FBB RID: 4027 RVA: 0x0005ED19 File Offset: 0x0005CF19
		private void OnAnimationCompleted(EntityUid uid, PoweredLightVisualsComponent comp, AnimationCompletedEvent args)
		{
			if (args.Key != "poweredlight_blinking")
			{
				return;
			}
			if (!comp.IsBlinking)
			{
				return;
			}
			this.AnimationSystem.Play(uid, base.Comp<AnimationPlayerComponent>(uid), this.BlinkingAnimation(comp), "poweredlight_blinking");
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x0005ED58 File Offset: 0x0005CF58
		private void SetBlinkingAnimation(EntityUid uid, bool shouldBeBlinking, PoweredLightVisualsComponent comp)
		{
			if (shouldBeBlinking == comp.IsBlinking)
			{
				return;
			}
			comp.IsBlinking = shouldBeBlinking;
			AnimationPlayerComponent animationPlayerComponent = base.EnsureComp<AnimationPlayerComponent>(uid);
			if (shouldBeBlinking)
			{
				this.AnimationSystem.Play(uid, animationPlayerComponent, this.BlinkingAnimation(comp), "poweredlight_blinking");
				return;
			}
			if (this.AnimationSystem.HasRunningAnimation(uid, animationPlayerComponent, "poweredlight_blinking"))
			{
				this.AnimationSystem.Stop(uid, animationPlayerComponent, "poweredlight_blinking");
			}
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x0005EDC4 File Offset: 0x0005CFC4
		private Animation BlinkingAnimation(PoweredLightVisualsComponent comp)
		{
			float num = MathHelper.Lerp(comp.MinBlinkingAnimationCycleTime, comp.MaxBlinkingAnimationCycleTime, this._random.NextFloat());
			Animation animation = new Animation
			{
				Length = TimeSpan.FromSeconds((double)num),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(PointLightComponent),
						InterpolationMode = 2,
						Property = "Enabled",
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(false, 0f),
							new AnimationTrackProperty.KeyFrame(true, 1f)
						}
					},
					new AnimationTrackSpriteFlick
					{
						LayerKey = PoweredLightLayers.Base,
						KeyFrames = 
						{
							new AnimationTrackSpriteFlick.KeyFrame(comp.SpriteStateMap[PoweredLightState.Off], 0f),
							new AnimationTrackSpriteFlick.KeyFrame(comp.SpriteStateMap[PoweredLightState.On], 0.5f)
						}
					}
				}
			};
			if (comp.BlinkingSound != null)
			{
				string sound = this._audio.GetSound(comp.BlinkingSound);
				animation.AnimationTracks.Add(new AnimationTrackPlaySound
				{
					KeyFrames = 
					{
						new AnimationTrackPlaySound.KeyFrame(sound, 0.5f, null)
					}
				});
			}
			return animation;
		}

		// Token: 0x040007BD RID: 1981
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040007BE RID: 1982
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
