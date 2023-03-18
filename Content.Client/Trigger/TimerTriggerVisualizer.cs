using System;
using System.Runtime.CompilerServices;
using Content.Shared.Trigger;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Trigger
{
	// Token: 0x020000EB RID: 235
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TimerTriggerVisualizer : AppearanceVisualizer, ISerializationHooks
	{
		// Token: 0x060006C7 RID: 1735 RVA: 0x00023B88 File Offset: 0x00021D88
		void ISerializationHooks.AfterDeserialization()
		{
			this.PrimingAnimation = new Animation
			{
				Length = TimeSpan.MaxValue
			};
			AnimationTrackSpriteFlick animationTrackSpriteFlick = new AnimationTrackSpriteFlick();
			this.PrimingAnimation.AnimationTracks.Add(animationTrackSpriteFlick);
			animationTrackSpriteFlick.LayerKey = TriggerVisualLayers.Base;
			animationTrackSpriteFlick.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("primed", 0f));
			if (this._countdownSound != null)
			{
				AnimationTrackPlaySound animationTrackPlaySound = new AnimationTrackPlaySound();
				this.PrimingAnimation.AnimationTracks.Add(animationTrackPlaySound);
				animationTrackPlaySound.KeyFrames.Add(new AnimationTrackPlaySound.KeyFrame(this._countdownSound.GetSound(null, null), 0f, null));
			}
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x00023C2F File Offset: 0x00021E2F
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			IoCManager.Resolve<IEntityManager>().EnsureComponent<AnimationPlayerComponent>(entity);
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x00023C40 File Offset: 0x00021E40
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			SpriteComponent component2 = entityManager.GetComponent<SpriteComponent>(component.Owner);
			AnimationPlayerComponent component3 = entityManager.GetComponent<AnimationPlayerComponent>(component.Owner);
			TriggerVisualState triggerVisualState;
			if (!component.TryGetData<TriggerVisualState>(TriggerVisuals.VisualState, ref triggerVisualState))
			{
				triggerVisualState = TriggerVisualState.Unprimed;
			}
			if (triggerVisualState != TriggerVisualState.Primed)
			{
				if (triggerVisualState != TriggerVisualState.Unprimed)
				{
					throw new ArgumentOutOfRangeException();
				}
				component2.LayerSetState(0, "icon");
				return;
			}
			else
			{
				if (!component3.HasRunningAnimation("priming_animation"))
				{
					component3.Play(this.PrimingAnimation, "priming_animation");
					return;
				}
				return;
			}
		}

		// Token: 0x0400031B RID: 795
		private const string AnimationKey = "priming_animation";

		// Token: 0x0400031C RID: 796
		[Nullable(2)]
		[DataField("countdown_sound", false, 1, false, false, null)]
		private SoundSpecifier _countdownSound;

		// Token: 0x0400031D RID: 797
		private Animation PrimingAnimation;
	}
}
