using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Actions
{
	// Token: 0x0200075A RID: 1882
	public sealed class GetItemActionsEvent : EntityEventArgs
	{
		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06001724 RID: 5924 RVA: 0x0004AE3F File Offset: 0x0004903F
		public bool InHands
		{
			get
			{
				return this.SlotFlags == null;
			}
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x0004AE4F File Offset: 0x0004904F
		public GetItemActionsEvent(SlotFlags? slotFlags = null)
		{
			this.SlotFlags = slotFlags;
		}

		// Token: 0x04001701 RID: 5889
		[Nullable(1)]
		public SortedSet<ActionType> Actions = new SortedSet<ActionType>();

		// Token: 0x04001702 RID: 5890
		public SlotFlags? SlotFlags;
	}
}
