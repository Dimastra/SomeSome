using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Power.Events
{
	// Token: 0x02000287 RID: 647
	public sealed class PowerPulseEvent : EntityEventArgs
	{
		// Token: 0x040007DA RID: 2010
		public EntityUid? User;

		// Token: 0x040007DB RID: 2011
		public EntityUid? Used;
	}
}
