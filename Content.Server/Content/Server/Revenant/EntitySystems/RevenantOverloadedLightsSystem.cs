using System;
using System.Runtime.CompilerServices;
using Content.Server.Beam;
using Content.Shared.Revenant.Components;
using Content.Shared.Revenant.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Revenant.EntitySystems
{
	// Token: 0x02000232 RID: 562
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RevenantOverloadedLightsSystem : SharedRevenantOverloadedLightsSystem
	{
		// Token: 0x06000B2D RID: 2861 RVA: 0x0003A574 File Offset: 0x00038774
		protected override void OnZap(RevenantOverloadedLightsComponent component)
		{
			if (component.Target == null)
			{
				return;
			}
			TransformComponent transformComponent = base.Transform(component.Owner);
			TransformComponent txform = base.Transform(component.Target.Value);
			float distance;
			if (!transformComponent.Coordinates.TryDistance(this.EntityManager, txform.Coordinates, ref distance))
			{
				return;
			}
			if (distance > component.ZapRange)
			{
				return;
			}
			this._beam.TryCreateBeam(component.Owner, component.Target.Value, component.ZapBeamEntityId, null, "unshaded", null);
		}

		// Token: 0x040006D2 RID: 1746
		[Dependency]
		private readonly BeamSystem _beam;
	}
}
