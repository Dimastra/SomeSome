using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Shared.Item
{
	// Token: 0x020003A7 RID: 935
	[Virtual]
	public class BasePickupAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000AAD RID: 2733 RVA: 0x00022CA8 File Offset: 0x00020EA8
		public BasePickupAttemptEvent(EntityUid user, EntityUid item)
		{
			this.User = user;
			this.Item = item;
		}

		// Token: 0x04000AAB RID: 2731
		public readonly EntityUid User;

		// Token: 0x04000AAC RID: 2732
		public readonly EntityUid Item;
	}
}
