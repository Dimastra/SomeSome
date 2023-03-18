using System;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;

namespace Content.Client.Singularity.Visualizers
{
	// Token: 0x02000140 RID: 320
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadiationCollectorVisualizer : AppearanceVisualizer, ISerializationHooks
	{
		// Token: 0x0600086A RID: 2154 RVA: 0x00030E4C File Offset: 0x0002F04C
		void ISerializationHooks.AfterDeserialization()
		{
			this.ActivateAnimation = new Animation
			{
				Length = TimeSpan.FromSeconds(0.800000011920929)
			};
			AnimationTrackSpriteFlick animationTrackSpriteFlick = new AnimationTrackSpriteFlick();
			this.ActivateAnimation.AnimationTracks.Add(animationTrackSpriteFlick);
			animationTrackSpriteFlick.LayerKey = RadiationCollectorVisualLayers.Main;
			animationTrackSpriteFlick.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("ca_active", 0f));
			this.DeactiveAnimation = new Animation
			{
				Length = TimeSpan.FromSeconds(0.800000011920929)
			};
			AnimationTrackSpriteFlick animationTrackSpriteFlick2 = new AnimationTrackSpriteFlick();
			this.DeactiveAnimation.AnimationTracks.Add(animationTrackSpriteFlick2);
			animationTrackSpriteFlick2.LayerKey = RadiationCollectorVisualLayers.Main;
			animationTrackSpriteFlick2.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("ca_deactive", 0f));
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x00023C2F File Offset: 0x00021E2F
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			IoCManager.Resolve<IEntityManager>().EnsureComponent<AnimationPlayerComponent>(entity);
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x00030F1C File Offset: 0x0002F11C
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			SpriteComponent spriteComponent;
			if (!entityManager.TryGetComponent<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				return;
			}
			AnimationPlayerComponent animationPlayerComponent;
			if (!entityManager.TryGetComponent<AnimationPlayerComponent>(component.Owner, ref animationPlayerComponent))
			{
				return;
			}
			RadiationCollectorVisualState radiationCollectorVisualState;
			if (!component.TryGetData<RadiationCollectorVisualState>(RadiationCollectorVisuals.VisualState, ref radiationCollectorVisualState))
			{
				radiationCollectorVisualState = RadiationCollectorVisualState.Deactive;
			}
			switch (radiationCollectorVisualState)
			{
			case RadiationCollectorVisualState.Active:
				spriteComponent.LayerSetState(RadiationCollectorVisualLayers.Main, "ca_on");
				return;
			case RadiationCollectorVisualState.Activating:
				if (!animationPlayerComponent.HasRunningAnimation("radiationcollector_animation"))
				{
					animationPlayerComponent.Play(this.ActivateAnimation, "radiationcollector_animation");
					animationPlayerComponent.AnimationCompleted += delegate(string _)
					{
						component.SetData(RadiationCollectorVisuals.VisualState, RadiationCollectorVisualState.Active);
					};
					return;
				}
				break;
			case RadiationCollectorVisualState.Deactivating:
				if (!animationPlayerComponent.HasRunningAnimation("radiationcollector_animation"))
				{
					animationPlayerComponent.Play(this.DeactiveAnimation, "radiationcollector_animation");
					animationPlayerComponent.AnimationCompleted += delegate(string _)
					{
						component.SetData(RadiationCollectorVisuals.VisualState, RadiationCollectorVisualState.Deactive);
					};
					return;
				}
				break;
			case RadiationCollectorVisualState.Deactive:
				spriteComponent.LayerSetState(RadiationCollectorVisualLayers.Main, "ca_off");
				break;
			default:
				return;
			}
		}

		// Token: 0x04000448 RID: 1096
		private const string AnimationKey = "radiationcollector_animation";

		// Token: 0x04000449 RID: 1097
		private Animation ActivateAnimation;

		// Token: 0x0400044A RID: 1098
		private Animation DeactiveAnimation;
	}
}
