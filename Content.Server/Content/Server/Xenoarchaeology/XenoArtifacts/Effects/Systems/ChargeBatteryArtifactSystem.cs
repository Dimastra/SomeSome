using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x0200003F RID: 63
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChargeBatteryArtifactSystem : EntitySystem
	{
		// Token: 0x060000C2 RID: 194 RVA: 0x00005C6E File Offset: 0x00003E6E
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ChargeBatteryArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<ChargeBatteryArtifactComponent, ArtifactActivatedEvent>(this.OnActivated), null, null);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00005C84 File Offset: 0x00003E84
		private void OnActivated(EntityUid uid, ChargeBatteryArtifactComponent component, ArtifactActivatedEvent args)
		{
			foreach (BatteryComponent batteryComponent in this._lookup.GetComponentsInRange<BatteryComponent>(base.Transform(uid).MapPosition, component.Radius))
			{
				batteryComponent.CurrentCharge = batteryComponent.MaxCharge;
			}
		}

		// Token: 0x0400008D RID: 141
		[Dependency]
		private readonly EntityLookupSystem _lookup;
	}
}
