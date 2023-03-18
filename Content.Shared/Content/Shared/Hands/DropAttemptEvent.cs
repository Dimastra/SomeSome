using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x02000427 RID: 1063
	public sealed class DropAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x04000C97 RID: 3223
		public readonly EntityUid Uid;
	}
}
