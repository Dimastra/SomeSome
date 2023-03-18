using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Beam.Components
{
	// Token: 0x02000678 RID: 1656
	public sealed class BeamControllerCreatedEvent : EntityEventArgs
	{
		// Token: 0x0600144B RID: 5195 RVA: 0x00043FD5 File Offset: 0x000421D5
		public BeamControllerCreatedEvent(EntityUid originBeam, EntityUid beamControllerEntity)
		{
			this.OriginBeam = originBeam;
			this.BeamControllerEntity = beamControllerEntity;
		}

		// Token: 0x040013EE RID: 5102
		public EntityUid OriginBeam;

		// Token: 0x040013EF RID: 5103
		public EntityUid BeamControllerEntity;
	}
}
