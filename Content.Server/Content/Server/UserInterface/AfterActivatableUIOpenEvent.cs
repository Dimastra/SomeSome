using System;
using System.Runtime.CompilerServices;
using Robust.Server.Player;
using Robust.Shared.GameObjects;

namespace Content.Server.UserInterface
{
	// Token: 0x020000F6 RID: 246
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AfterActivatableUIOpenEvent : EntityEventArgs
	{
		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x0600048C RID: 1164 RVA: 0x00015ECC File Offset: 0x000140CC
		public EntityUid User { get; }

		// Token: 0x0600048D RID: 1165 RVA: 0x00015ED4 File Offset: 0x000140D4
		public AfterActivatableUIOpenEvent(EntityUid who, IPlayerSession session)
		{
			this.User = who;
			this.Session = session;
		}

		// Token: 0x040002AC RID: 684
		public readonly IPlayerSession Session;
	}
}
