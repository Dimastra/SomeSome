using System;
using System.Runtime.CompilerServices;
using Robust.Server.Player;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking
{
	// Token: 0x020004A9 RID: 1193
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PlayerJoinedLobbyEvent : EntityEventArgs
	{
		// Token: 0x06001863 RID: 6243 RVA: 0x0007F737 File Offset: 0x0007D937
		public PlayerJoinedLobbyEvent(IPlayerSession playerSession)
		{
			this.PlayerSession = playerSession;
		}

		// Token: 0x04000F23 RID: 3875
		public readonly IPlayerSession PlayerSession;
	}
}
