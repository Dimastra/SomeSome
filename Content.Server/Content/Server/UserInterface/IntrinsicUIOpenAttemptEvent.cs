using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.UserInterface
{
	// Token: 0x020000FD RID: 253
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class IntrinsicUIOpenAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060004A0 RID: 1184 RVA: 0x000161B3 File Offset: 0x000143B3
		public EntityUid User { get; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060004A1 RID: 1185 RVA: 0x000161BB File Offset: 0x000143BB
		public Enum Key { get; }

		// Token: 0x060004A2 RID: 1186 RVA: 0x000161C3 File Offset: 0x000143C3
		public IntrinsicUIOpenAttemptEvent(EntityUid who, Enum key)
		{
			this.User = who;
			this.Key = key;
		}
	}
}
