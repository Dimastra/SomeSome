using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing
{
	// Token: 0x020000D4 RID: 212
	public sealed class ThrowHitByEvent : ThrowEvent
	{
		// Token: 0x06000251 RID: 593 RVA: 0x0000B27E File Offset: 0x0000947E
		public ThrowHitByEvent(EntityUid? user, EntityUid thrown, EntityUid target) : base(user, thrown, target)
		{
		}
	}
}
