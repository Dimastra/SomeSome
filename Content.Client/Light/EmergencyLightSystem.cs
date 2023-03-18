using System;
using System.Runtime.CompilerServices;
using Content.Client.Light.Components;
using Content.Shared.Light;
using Content.Shared.Light.Component;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;

namespace Content.Client.Light
{
	// Token: 0x02000261 RID: 609
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EmergencyLightSystem : SharedEmergencyLightSystem
	{
		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06000F9A RID: 3994 RVA: 0x0005E004 File Offset: 0x0005C204
		private static Animation Animation
		{
			get
			{
				return new Animation
				{
					Length = TimeSpan.FromSeconds(4.0),
					AnimationTracks = 
					{
						new AnimationTrackComponentProperty
						{
							ComponentType = typeof(PointLightComponent),
							InterpolationMode = 0,
							Property = "Rotation",
							KeyFrames = 
							{
								new AnimationTrackProperty.KeyFrame(Angle.Zero, 0f),
								new AnimationTrackProperty.KeyFrame(Angle.FromDegrees(120.0), 1.3333334f),
								new AnimationTrackProperty.KeyFrame(Angle.FromDegrees(240.0), 1.3333334f),
								new AnimationTrackProperty.KeyFrame(Angle.FromDegrees(360.0), 1.3333334f)
							}
						}
					}
				};
			}
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x0005E0F4 File Offset: 0x0005C2F4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EmergencyLightComponent, ComponentStartup>(new ComponentEventHandler<EmergencyLightComponent, ComponentStartup>(this.HandleStartup), null, null);
			base.SubscribeLocalEvent<EmergencyLightComponent, AnimationCompletedEvent>(new ComponentEventHandler<EmergencyLightComponent, AnimationCompletedEvent>(this.HandleAnimationComplete), null, null);
			base.SubscribeLocalEvent<EmergencyLightComponent, ComponentHandleState>(new ComponentEventRefHandler<EmergencyLightComponent, ComponentHandleState>(this.HandleCompState), null, null);
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x0005E144 File Offset: 0x0005C344
		private void HandleCompState(EntityUid uid, EmergencyLightComponent component, ref ComponentHandleState args)
		{
			EmergencyLightComponentState emergencyLightComponentState = args.Current as EmergencyLightComponentState;
			if (emergencyLightComponentState == null)
			{
				return;
			}
			if (component.Enabled == emergencyLightComponentState.Enabled)
			{
				return;
			}
			AnimationPlayerComponent animationPlayerComponent = ComponentExt.EnsureComponent<AnimationPlayerComponent>(component.Owner);
			component.Enabled = emergencyLightComponentState.Enabled;
			if (component.Enabled && !animationPlayerComponent.HasRunningAnimation("emergency"))
			{
				animationPlayerComponent.Play(EmergencyLightSystem.Animation, "emergency");
			}
			if (!component.Enabled)
			{
				animationPlayerComponent.Stop("emergency");
			}
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x0005E1C0 File Offset: 0x0005C3C0
		private void HandleAnimationComplete(EntityUid uid, EmergencyLightComponent component, AnimationCompletedEvent args)
		{
			AnimationPlayerComponent animationPlayerComponent;
			if (!component.Enabled || !this.EntityManager.TryGetComponent<AnimationPlayerComponent>(uid, ref animationPlayerComponent))
			{
				return;
			}
			animationPlayerComponent.Play(EmergencyLightSystem.Animation, "emergency");
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x0005E1F6 File Offset: 0x0005C3F6
		private void HandleStartup(EntityUid uid, EmergencyLightComponent component, ComponentStartup args)
		{
			this.PlayAnimation(component);
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x0005E200 File Offset: 0x0005C400
		private void PlayAnimation(EmergencyLightComponent component)
		{
			if (!component.Enabled)
			{
				return;
			}
			AnimationPlayerComponent animationPlayerComponent = ComponentExt.EnsureComponent<AnimationPlayerComponent>(component.Owner);
			if (!animationPlayerComponent.HasRunningAnimation("emergency"))
			{
				animationPlayerComponent.Play(EmergencyLightSystem.Animation, "emergency");
			}
		}

		// Token: 0x040007B5 RID: 1973
		private const float DegreesPerSecond = 90f;

		// Token: 0x040007B6 RID: 1974
		private const string AnimKey = "emergency";
	}
}
