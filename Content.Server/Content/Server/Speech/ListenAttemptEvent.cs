using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Speech
{
	// Token: 0x020001B0 RID: 432
	public sealed class ListenAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x0600087E RID: 2174 RVA: 0x0002B500 File Offset: 0x00029700
		public ListenAttemptEvent(EntityUid source)
		{
			this.Source = source;
		}

		// Token: 0x04000531 RID: 1329
		public readonly EntityUid Source;
	}
}
