using System;
using System.Runtime.CompilerServices;
using Robust.Server.Player;

namespace Content.Server.Players
{
	// Token: 0x020002D2 RID: 722
	[NullableContext(1)]
	[Nullable(0)]
	public static class PlayerDataExt
	{
		// Token: 0x06000E89 RID: 3721 RVA: 0x00049F81 File Offset: 0x00048181
		[return: Nullable(2)]
		public static PlayerData ContentData(this IPlayerData data)
		{
			return (PlayerData)data.ContentDataUncast;
		}

		// Token: 0x06000E8A RID: 3722 RVA: 0x00049F8E File Offset: 0x0004818E
		[return: Nullable(2)]
		public static PlayerData ContentData(this IPlayerSession session)
		{
			return session.Data.ContentData();
		}
	}
}
