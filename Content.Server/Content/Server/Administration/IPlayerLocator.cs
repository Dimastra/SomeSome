using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.Network;

namespace Content.Server.Administration
{
	// Token: 0x02000802 RID: 2050
	[NullableContext(1)]
	public interface IPlayerLocator
	{
		// Token: 0x06002C66 RID: 11366
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<LocatedPlayerData> LookupIdByNameAsync(string playerName, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06002C67 RID: 11367
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<LocatedPlayerData> LookupIdByNameOrIdAsync(string playerName, CancellationToken cancel = default(CancellationToken));

		// Token: 0x06002C68 RID: 11368
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		Task<LocatedPlayerData> LookupIdAsync(NetUserId userId, CancellationToken cancel = default(CancellationToken));
	}
}
