using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking
{
	// Token: 0x0200046A RID: 1130
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class TickerLobbyReadyEvent : EntityEventArgs
	{
		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06000DB3 RID: 3507 RVA: 0x0002CAD1 File Offset: 0x0002ACD1
		public Dictionary<NetUserId, PlayerGameStatus> Status { get; }

		// Token: 0x06000DB4 RID: 3508 RVA: 0x0002CAD9 File Offset: 0x0002ACD9
		public TickerLobbyReadyEvent(Dictionary<NetUserId, PlayerGameStatus> status)
		{
			this.Status = status;
		}
	}
}
