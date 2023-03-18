using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Beam.Components
{
	// Token: 0x0200067A RID: 1658
	public sealed class BeamFiredEvent : EntityEventArgs
	{
		// Token: 0x0600144D RID: 5197 RVA: 0x00044001 File Offset: 0x00042201
		public BeamFiredEvent(EntityUid createdBeam)
		{
			this.CreatedBeam = createdBeam;
		}

		// Token: 0x040013F2 RID: 5106
		public readonly EntityUid CreatedBeam;
	}
}
