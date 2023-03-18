using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing
{
	// Token: 0x020000D5 RID: 213
	public sealed class ThrowDoHitEvent : ThrowEvent
	{
		// Token: 0x06000252 RID: 594 RVA: 0x0000B289 File Offset: 0x00009489
		public ThrowDoHitEvent(EntityUid? user, EntityUid thrown, EntityUid target) : base(user, thrown, target)
		{
		}
	}
}
