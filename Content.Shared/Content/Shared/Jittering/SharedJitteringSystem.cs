using System;
using System.Runtime.CompilerServices;
using Content.Shared.Rejuvenate;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Jittering
{
	// Token: 0x0200039F RID: 927
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedJitteringSystem : EntitySystem
	{
		// Token: 0x06000A9E RID: 2718 RVA: 0x00022AA1 File Offset: 0x00020CA1
		public override void Initialize()
		{
			base.SubscribeLocalEvent<JitteringComponent, ComponentGetState>(new ComponentEventRefHandler<JitteringComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<JitteringComponent, ComponentHandleState>(new ComponentEventRefHandler<JitteringComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<JitteringComponent, RejuvenateEvent>(new ComponentEventHandler<JitteringComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x00022ADF File Offset: 0x00020CDF
		private void OnGetState(EntityUid uid, JitteringComponent component, ref ComponentGetState args)
		{
			args.State = new JitteringComponentState(component.Amplitude, component.Frequency);
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00022AF8 File Offset: 0x00020CF8
		private void OnHandleState(EntityUid uid, JitteringComponent component, ref ComponentHandleState args)
		{
			JitteringComponentState jitteringState = args.Current as JitteringComponentState;
			if (jitteringState == null)
			{
				return;
			}
			component.Amplitude = jitteringState.Amplitude;
			component.Frequency = jitteringState.Frequency;
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00022B2D File Offset: 0x00020D2D
		private void OnRejuvenate(EntityUid uid, JitteringComponent component, RejuvenateEvent args)
		{
			this.EntityManager.RemoveComponentDeferred<JitteringComponent>(uid);
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x00022B3C File Offset: 0x00020D3C
		[NullableContext(2)]
		public void DoJitter(EntityUid uid, TimeSpan time, bool refresh, float amplitude = 10f, float frequency = 4f, bool forceValueChange = false, StatusEffectsComponent status = null)
		{
			if (!base.Resolve<StatusEffectsComponent>(uid, ref status, false))
			{
				return;
			}
			amplitude = Math.Clamp(amplitude, this.MinAmplitude, this.MaxAmplitude);
			frequency = Math.Clamp(frequency, this.MinFrequency, this.MaxFrequency);
			if (this.StatusEffects.TryAddStatusEffect<JitteringComponent>(uid, "Jitter", time, refresh, status))
			{
				JitteringComponent jittering = this.EntityManager.GetComponent<JitteringComponent>(uid);
				if (forceValueChange || jittering.Amplitude < amplitude)
				{
					jittering.Amplitude = amplitude;
				}
				if (forceValueChange || jittering.Frequency < frequency)
				{
					jittering.Frequency = frequency;
				}
			}
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x00022BD0 File Offset: 0x00020DD0
		public void AddJitter(EntityUid uid, float amplitude = 10f, float frequency = 4f)
		{
			JitteringComponent jitter = base.EnsureComp<JitteringComponent>(uid);
			jitter.Amplitude = amplitude;
			jitter.Frequency = frequency;
			base.Dirty(jitter, null);
		}

		// Token: 0x04000A94 RID: 2708
		[Dependency]
		protected readonly IGameTiming GameTiming;

		// Token: 0x04000A95 RID: 2709
		[Dependency]
		protected readonly StatusEffectsSystem StatusEffects;

		// Token: 0x04000A96 RID: 2710
		public float MaxAmplitude = 300f;

		// Token: 0x04000A97 RID: 2711
		public float MinAmplitude = 1f;

		// Token: 0x04000A98 RID: 2712
		public float MaxFrequency = 10f;

		// Token: 0x04000A99 RID: 2713
		public float MinFrequency = 1f;
	}
}
