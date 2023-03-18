using System;
using System.Runtime.CompilerServices;
using Content.Shared.Bed.Sleep;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Traits.Assorted
{
	// Token: 0x02000104 RID: 260
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NarcolepsySystem : EntitySystem
	{
		// Token: 0x060004B7 RID: 1207 RVA: 0x000168FC File Offset: 0x00014AFC
		public override void Initialize()
		{
			base.SubscribeLocalEvent<NarcolepsyComponent, ComponentStartup>(new ComponentEventHandler<NarcolepsyComponent, ComponentStartup>(this.SetupNarcolepsy), null, null);
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x00016912 File Offset: 0x00014B12
		private void SetupNarcolepsy(EntityUid uid, NarcolepsyComponent component, ComponentStartup args)
		{
			component.NextIncidentTime = this._random.NextFloat(component.TimeBetweenIncidents.X, component.TimeBetweenIncidents.Y);
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x0001693B File Offset: 0x00014B3B
		[NullableContext(2)]
		public void AdjustNarcolepsyTimer(EntityUid uid, int TimerReset, NarcolepsyComponent narcolepsy = null)
		{
			if (!base.Resolve<NarcolepsyComponent>(uid, ref narcolepsy, false))
			{
				return;
			}
			narcolepsy.NextIncidentTime = (float)TimerReset;
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x00016954 File Offset: 0x00014B54
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (NarcolepsyComponent narcolepsy in base.EntityQuery<NarcolepsyComponent>(false))
			{
				narcolepsy.NextIncidentTime -= frameTime;
				if (narcolepsy.NextIncidentTime < 0f)
				{
					narcolepsy.NextIncidentTime += this._random.NextFloat(narcolepsy.TimeBetweenIncidents.X, narcolepsy.TimeBetweenIncidents.Y);
					float duration = this._random.NextFloat(narcolepsy.DurationOfIncident.X, narcolepsy.DurationOfIncident.Y);
					narcolepsy.NextIncidentTime += duration;
					this._statusEffects.TryAddStatusEffect<ForcedSleepingComponent>(narcolepsy.Owner, "ForcedSleep", TimeSpan.FromSeconds((double)duration), false, null);
				}
			}
		}

		// Token: 0x040002BD RID: 701
		private const string StatusEffectKey = "ForcedSleep";

		// Token: 0x040002BE RID: 702
		[Dependency]
		private readonly StatusEffectsSystem _statusEffects;

		// Token: 0x040002BF RID: 703
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
