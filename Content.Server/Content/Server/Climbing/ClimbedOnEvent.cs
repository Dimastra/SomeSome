using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Climbing
{
	// Token: 0x02000647 RID: 1607
	public sealed class ClimbedOnEvent : EntityEventArgs
	{
		// Token: 0x06002229 RID: 8745 RVA: 0x000B2DF8 File Offset: 0x000B0FF8
		public ClimbedOnEvent(EntityUid climber, EntityUid instigator)
		{
			this.Climber = climber;
			this.Instigator = instigator;
		}

		// Token: 0x04001513 RID: 5395
		public EntityUid Climber;

		// Token: 0x04001514 RID: 5396
		public EntityUid Instigator;
	}
}
