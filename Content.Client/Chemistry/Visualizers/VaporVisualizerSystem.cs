using System;
using System.Runtime.CompilerServices;
using Content.Shared.Vapor;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Chemistry.Visualizers
{
	// Token: 0x020003CD RID: 973
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class VaporVisualizerSystem : VisualizerSystem<VaporVisualsComponent>
	{
		// Token: 0x060017F8 RID: 6136 RVA: 0x00089D4F File Offset: 0x00087F4F
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<VaporVisualsComponent, ComponentInit>(new ComponentEventHandler<VaporVisualsComponent, ComponentInit>(this.OnComponentInit), null, null);
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x00089D6C File Offset: 0x00087F6C
		private void OnComponentInit(EntityUid uid, VaporVisualsComponent comp, ComponentInit args)
		{
			comp.VaporFlick = new Animation
			{
				Length = TimeSpan.FromSeconds((double)comp.AnimationTime),
				AnimationTracks = 
				{
					new AnimationTrackSpriteFlick
					{
						LayerKey = VaporVisualLayers.Base,
						KeyFrames = 
						{
							new AnimationTrackSpriteFlick.KeyFrame(comp.AnimationState, 0f)
						}
					}
				}
			};
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x00089DD4 File Offset: 0x00087FD4
		protected override void OnAppearanceChange(EntityUid uid, VaporVisualsComponent comp, ref AppearanceChangeEvent args)
		{
			Color color;
			if (this.AppearanceSystem.TryGetData<Color>(uid, VaporVisuals.Color, ref color, args.Component) && args.Sprite != null)
			{
				args.Sprite.Color = color;
			}
			bool flag;
			AnimationPlayerComponent animationPlayerComponent;
			if (this.AppearanceSystem.TryGetData<bool>(uid, VaporVisuals.State, ref flag, args.Component) && flag && base.TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent) && !this.AnimationSystem.HasRunningAnimation(uid, animationPlayerComponent, "flick_animation"))
			{
				this.AnimationSystem.Play(uid, animationPlayerComponent, comp.VaporFlick, "flick_animation");
			}
		}
	}
}
