using System;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking.Events
{
	// Token: 0x020004D1 RID: 1233
	public sealed class RoundStartingEvent : EntityEventArgs
	{
		// Token: 0x06001974 RID: 6516 RVA: 0x000861B8 File Offset: 0x000843B8
		public RoundStartingEvent(int id)
		{
			this.Id = id;
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06001975 RID: 6517 RVA: 0x000861C7 File Offset: 0x000843C7
		public int Id { get; }
	}
}
