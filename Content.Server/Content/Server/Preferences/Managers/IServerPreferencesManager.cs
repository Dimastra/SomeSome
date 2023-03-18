using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Shared.Preferences;
using Robust.Server.Player;
using Robust.Shared.Network;

namespace Content.Server.Preferences.Managers
{
	// Token: 0x0200026D RID: 621
	[NullableContext(1)]
	public interface IServerPreferencesManager
	{
		// Token: 0x06000C60 RID: 3168
		void Init();

		// Token: 0x06000C61 RID: 3169
		Task LoadData(IPlayerSession session, CancellationToken cancel);

		// Token: 0x06000C62 RID: 3170
		void OnClientDisconnected(IPlayerSession session);

		// Token: 0x06000C63 RID: 3171
		[NullableContext(2)]
		bool TryGetCachedPreferences(NetUserId userId, [NotNullWhen(true)] out PlayerPreferences playerPreferences);

		// Token: 0x06000C64 RID: 3172
		PlayerPreferences GetPreferences(NetUserId userId);

		// Token: 0x06000C65 RID: 3173
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		IEnumerable<KeyValuePair<NetUserId, ICharacterProfile>> GetSelectedProfilesForPlayers(List<NetUserId> userIds);
	}
}
