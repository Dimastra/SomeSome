using System;
using System.Runtime.CompilerServices;
using Content.Shared.Trigger;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Explosion
{
	// Token: 0x02000327 RID: 807
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TriggerSystem : EntitySystem
	{
		// Token: 0x06001444 RID: 5188 RVA: 0x00076E82 File Offset: 0x00075082
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeProximity();
		}

		// Token: 0x06001445 RID: 5189 RVA: 0x00076E90 File Offset: 0x00075090
		private void InitializeProximity()
		{
			base.SubscribeLocalEvent<TriggerOnProximityComponent, ComponentInit>(new ComponentEventHandler<TriggerOnProximityComponent, ComponentInit>(this.OnProximityInit), null, null);
			base.SubscribeLocalEvent<TriggerOnProximityComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<TriggerOnProximityComponent, AppearanceChangeEvent>(this.OnProxAppChange), null, null);
			base.SubscribeLocalEvent<TriggerOnProximityComponent, AnimationCompletedEvent>(new ComponentEventHandler<TriggerOnProximityComponent, AnimationCompletedEvent>(this.OnProxAnimation), null, null);
		}

		// Token: 0x06001446 RID: 5190 RVA: 0x00076ED0 File Offset: 0x000750D0
		private void OnProxAnimation(EntityUid uid, TriggerOnProximityComponent component, AnimationCompletedEvent args)
		{
			AppearanceComponent appearanceComponent;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearanceComponent))
			{
				return;
			}
			this._appearance.SetData(uid, ProximityTriggerVisualState.State, ProximityTriggerVisuals.Inactive, appearanceComponent);
			this.OnChangeData(uid, component, appearanceComponent, null);
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x00076F0C File Offset: 0x0007510C
		private void OnProximityInit(EntityUid uid, TriggerOnProximityComponent component, ComponentInit args)
		{
			this.EntityManager.EnsureComponent<AnimationPlayerComponent>(uid);
		}

		// Token: 0x06001448 RID: 5192 RVA: 0x00076F1B File Offset: 0x0007511B
		private void OnProxAppChange(EntityUid uid, TriggerOnProximityComponent component, ref AppearanceChangeEvent args)
		{
			this.OnChangeData(uid, component, args.Component, args.Sprite);
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x00076F34 File Offset: 0x00075134
		private void OnChangeData(EntityUid uid, TriggerOnProximityComponent component, AppearanceComponent appearance, [Nullable(2)] SpriteComponent spriteComponent = null)
		{
			if (!base.Resolve<SpriteComponent>(uid, ref spriteComponent, true))
			{
				return;
			}
			AnimationPlayerComponent animationPlayerComponent;
			base.TryComp<AnimationPlayerComponent>(component.Owner, ref animationPlayerComponent);
			ProximityTriggerVisuals proximityTriggerVisuals;
			this._appearance.TryGetData<ProximityTriggerVisuals>(appearance.Owner, ProximityTriggerVisualState.State, ref proximityTriggerVisuals, appearance);
			switch (proximityTriggerVisuals)
			{
			case ProximityTriggerVisuals.Inactive:
				if (this._player.HasRunningAnimation(uid, animationPlayerComponent, "proximity"))
				{
					return;
				}
				this._player.Stop(uid, animationPlayerComponent, "proximity");
				spriteComponent.LayerSetState(TriggerSystem.ProximityTriggerVisualLayers.Base, "on");
				return;
			case ProximityTriggerVisuals.Active:
				if (this._player.HasRunningAnimation(uid, animationPlayerComponent, "proximity"))
				{
					return;
				}
				this._player.Play(uid, animationPlayerComponent, TriggerSystem._flasherAnimation, "proximity");
				return;
			}
			this._player.Stop(uid, animationPlayerComponent, "proximity");
			spriteComponent.LayerSetState(TriggerSystem.ProximityTriggerVisualLayers.Base, "off");
		}

		// Token: 0x04000A34 RID: 2612
		[Dependency]
		private readonly AnimationPlayerSystem _player;

		// Token: 0x04000A35 RID: 2613
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000A36 RID: 2614
		private const string AnimKey = "proximity";

		// Token: 0x04000A37 RID: 2615
		private static readonly Animation _flasherAnimation = new Animation
		{
			Length = TimeSpan.FromSeconds(0.30000001192092896),
			AnimationTracks = 
			{
				new AnimationTrackSpriteFlick
				{
					LayerKey = TriggerSystem.ProximityTriggerVisualLayers.Base,
					KeyFrames = 
					{
						new AnimationTrackSpriteFlick.KeyFrame("flashing", 0f)
					}
				},
				new AnimationTrackComponentProperty
				{
					ComponentType = typeof(PointLightComponent),
					InterpolationMode = 2,
					Property = "Radius",
					KeyFrames = 
					{
						new AnimationTrackProperty.KeyFrame(0.1f, 0f),
						new AnimationTrackProperty.KeyFrame(3f, 0.1f),
						new AnimationTrackProperty.KeyFrame(0.1f, 0.5f)
					}
				}
			}
		};

		// Token: 0x02000328 RID: 808
		[NullableContext(0)]
		public enum ProximityTriggerVisualLayers : byte
		{
			// Token: 0x04000A39 RID: 2617
			Base
		}
	}
}
