using System;
using System.Runtime.CompilerServices;
using Content.Shared.Foam;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Chemistry.Visualizers
{
	// Token: 0x020003C6 RID: 966
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class FoamVisualizerSystem : VisualizerSystem<FoamVisualsComponent>
	{
		// Token: 0x060017EB RID: 6123 RVA: 0x00089897 File Offset: 0x00087A97
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FoamVisualsComponent, ComponentInit>(new ComponentEventHandler<FoamVisualsComponent, ComponentInit>(this.OnComponentInit), null, null);
		}

		// Token: 0x060017EC RID: 6124 RVA: 0x000898B4 File Offset: 0x00087AB4
		private void OnComponentInit(EntityUid uid, FoamVisualsComponent comp, ComponentInit args)
		{
			comp.Animation = new Animation
			{
				Length = TimeSpan.FromSeconds((double)comp.AnimationTime),
				AnimationTracks = 
				{
					new AnimationTrackSpriteFlick
					{
						LayerKey = FoamVisualLayers.Base,
						KeyFrames = 
						{
							new AnimationTrackSpriteFlick.KeyFrame(comp.State, 0f)
						}
					}
				}
			};
		}

		// Token: 0x060017ED RID: 6125 RVA: 0x0008991C File Offset: 0x00087B1C
		protected override void OnAppearanceChange(EntityUid uid, FoamVisualsComponent comp, ref AppearanceChangeEvent args)
		{
			bool flag;
			AnimationPlayerComponent animationPlayerComponent;
			if (this.AppearanceSystem.TryGetData<bool>(uid, FoamVisuals.State, ref flag, args.Component) && flag && base.TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent) && !this.AnimationSystem.HasRunningAnimation(uid, animationPlayerComponent, "foamdissolve_animation"))
			{
				this.AnimationSystem.Play(uid, animationPlayerComponent, comp.Animation, "foamdissolve_animation");
			}
			Color color;
			if (this.AppearanceSystem.TryGetData<Color>(uid, FoamVisuals.Color, ref color, args.Component) && args.Sprite != null)
			{
				args.Sprite.Color = color;
			}
		}
	}
}
