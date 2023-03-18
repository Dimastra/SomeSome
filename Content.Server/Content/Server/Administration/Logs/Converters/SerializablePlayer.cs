using System;
using System.Runtime.CompilerServices;
using Robust.Server.Player;

namespace Content.Server.Administration.Logs.Converters
{
	// Token: 0x02000825 RID: 2085
	[NullableContext(1)]
	[Nullable(0)]
	public readonly struct SerializablePlayer
	{
		// Token: 0x06002DC6 RID: 11718 RVA: 0x000EFD3A File Offset: 0x000EDF3A
		public SerializablePlayer(IPlayerSession player)
		{
			this.Player = player;
		}

		// Token: 0x04001C43 RID: 7235
		public readonly IPlayerSession Player;
	}
}
