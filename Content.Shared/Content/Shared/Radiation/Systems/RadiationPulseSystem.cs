using System;
using System.Runtime.CompilerServices;
using Content.Shared.Radiation.Components;
using Content.Shared.Spawners.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Radiation.Systems
{
	// Token: 0x02000227 RID: 551
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadiationPulseSystem : EntitySystem
	{
		// Token: 0x06000623 RID: 1571 RVA: 0x00015B0A File Offset: 0x00013D0A
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RadiationPulseComponent, ComponentStartup>(new ComponentEventHandler<RadiationPulseComponent, ComponentStartup>(this.OnStartup), null, null);
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x00015B28 File Offset: 0x00013D28
		private void OnStartup(EntityUid uid, RadiationPulseComponent component, ComponentStartup args)
		{
			component.StartTime = this._timing.RealTime;
			TimedDespawnComponent despawn;
			if (base.TryComp<TimedDespawnComponent>(uid, ref despawn))
			{
				component.VisualDuration = despawn.Lifetime;
			}
			RadiationSourceComponent radSource;
			if (base.TryComp<RadiationSourceComponent>(uid, ref radSource))
			{
				component.VisualRange = radSource.Intensity / radSource.Slope;
			}
		}

		// Token: 0x04000628 RID: 1576
		[Dependency]
		private readonly IGameTiming _timing;
	}
}
