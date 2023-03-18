using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech.Components
{
	// Token: 0x0200032B RID: 811
	[NetSerializable]
	[Serializable]
	public sealed class MechComponentState : ComponentState
	{
		// Token: 0x0400093C RID: 2364
		public FixedPoint2 Integrity;

		// Token: 0x0400093D RID: 2365
		public FixedPoint2 MaxIntegrity;

		// Token: 0x0400093E RID: 2366
		public FixedPoint2 Energy;

		// Token: 0x0400093F RID: 2367
		public FixedPoint2 MaxEnergy;

		// Token: 0x04000940 RID: 2368
		public EntityUid? CurrentSelectedEquipment;

		// Token: 0x04000941 RID: 2369
		public bool Broken;
	}
}
