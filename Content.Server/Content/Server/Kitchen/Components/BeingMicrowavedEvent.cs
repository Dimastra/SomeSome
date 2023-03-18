using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Kitchen.Components
{
	// Token: 0x02000436 RID: 1078
	public sealed class BeingMicrowavedEvent : HandledEntityEventArgs
	{
		// Token: 0x060015E5 RID: 5605 RVA: 0x0007402E File Offset: 0x0007222E
		public BeingMicrowavedEvent(EntityUid microwave, EntityUid? user)
		{
			this.Microwave = microwave;
			this.User = user;
		}

		// Token: 0x04000DB1 RID: 3505
		public EntityUid Microwave;

		// Token: 0x04000DB2 RID: 3506
		public EntityUid? User;
	}
}
