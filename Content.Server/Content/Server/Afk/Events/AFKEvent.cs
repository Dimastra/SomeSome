using System;
using System.Runtime.CompilerServices;
using Robust.Server.Player;
using Robust.Shared.GameObjects;

namespace Content.Server.Afk.Events
{
	// Token: 0x020007F6 RID: 2038
	[NullableContext(1)]
	[Nullable(0)]
	[ByRefEvent]
	public readonly struct AFKEvent
	{
		// Token: 0x06002C18 RID: 11288 RVA: 0x000E6D30 File Offset: 0x000E4F30
		public AFKEvent(IPlayerSession playerSession)
		{
			this.Session = playerSession;
		}

		// Token: 0x04001B51 RID: 6993
		public readonly IPlayerSession Session;
	}
}
