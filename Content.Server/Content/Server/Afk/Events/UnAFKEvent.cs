using System;
using System.Runtime.CompilerServices;
using Robust.Server.Player;
using Robust.Shared.GameObjects;

namespace Content.Server.Afk.Events
{
	// Token: 0x020007F7 RID: 2039
	[NullableContext(1)]
	[Nullable(0)]
	[ByRefEvent]
	public readonly struct UnAFKEvent
	{
		// Token: 0x06002C19 RID: 11289 RVA: 0x000E6D39 File Offset: 0x000E4F39
		public UnAFKEvent(IPlayerSession playerSession)
		{
			this.Session = playerSession;
		}

		// Token: 0x04001B52 RID: 6994
		public readonly IPlayerSession Session;
	}
}
