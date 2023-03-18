using System;
using Content.Shared.Actions;
using Robust.Shared.GameObjects;

namespace Content.Server.Borgs
{
	// Token: 0x020000A1 RID: 161
	public sealed class InnateAfterInteractActionEvent : EntityTargetActionEvent
	{
		// Token: 0x06000285 RID: 645 RVA: 0x0000D8F2 File Offset: 0x0000BAF2
		public InnateAfterInteractActionEvent(EntityUid item)
		{
			this.Item = item;
		}

		// Token: 0x040001D5 RID: 469
		public EntityUid Item;
	}
}
