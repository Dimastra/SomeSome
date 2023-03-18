using System;
using Robust.Shared.GameObjects;

namespace Content.Server.UserInterface
{
	// Token: 0x020000F7 RID: 247
	public sealed class BeforeActivatableUIOpenEvent : EntityEventArgs
	{
		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x0600048E RID: 1166 RVA: 0x00015EEA File Offset: 0x000140EA
		public EntityUid User { get; }

		// Token: 0x0600048F RID: 1167 RVA: 0x00015EF2 File Offset: 0x000140F2
		public BeforeActivatableUIOpenEvent(EntityUid who)
		{
			this.User = who;
		}
	}
}
