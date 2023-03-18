using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Item
{
	// Token: 0x020003A6 RID: 934
	public sealed class GettingPickedUpAttemptEvent : BasePickupAttemptEvent
	{
		// Token: 0x06000AAC RID: 2732 RVA: 0x00022C9E File Offset: 0x00020E9E
		public GettingPickedUpAttemptEvent(EntityUid user, EntityUid item) : base(user, item)
		{
		}
	}
}
