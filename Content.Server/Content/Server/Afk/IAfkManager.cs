using System;
using System.Runtime.CompilerServices;
using Robust.Server.Player;

namespace Content.Server.Afk
{
	// Token: 0x020007F2 RID: 2034
	[NullableContext(1)]
	public interface IAfkManager
	{
		// Token: 0x06002C02 RID: 11266
		bool IsAfk(IPlayerSession player);

		// Token: 0x06002C03 RID: 11267
		void PlayerDidAction(IPlayerSession player);

		// Token: 0x06002C04 RID: 11268
		void Initialize();
	}
}
