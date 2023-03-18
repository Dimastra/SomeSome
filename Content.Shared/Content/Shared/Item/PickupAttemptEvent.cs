using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Item
{
	// Token: 0x020003A5 RID: 933
	public sealed class PickupAttemptEvent : BasePickupAttemptEvent
	{
		// Token: 0x06000AAB RID: 2731 RVA: 0x00022C94 File Offset: 0x00020E94
		public PickupAttemptEvent(EntityUid user, EntityUid item) : base(user, item)
		{
		}
	}
}
