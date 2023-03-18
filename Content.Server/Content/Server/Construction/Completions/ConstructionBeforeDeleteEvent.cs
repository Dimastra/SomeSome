using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000615 RID: 1557
	public sealed class ConstructionBeforeDeleteEvent : CancellableEntityEventArgs
	{
		// Token: 0x06002149 RID: 8521 RVA: 0x000AE57E File Offset: 0x000AC77E
		public ConstructionBeforeDeleteEvent(EntityUid? user)
		{
			this.User = user;
		}

		// Token: 0x0400147D RID: 5245
		public EntityUid? User;
	}
}
