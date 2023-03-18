using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Preferences;
using Robust.Server.Player;
using Robust.Shared.Network;

namespace Content.Server.GameTicking
{
	// Token: 0x020004B1 RID: 1201
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RulePlayerSpawningEvent
	{
		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06001871 RID: 6257 RVA: 0x0007F805 File Offset: 0x0007DA05
		public List<IPlayerSession> PlayerPool { get; }

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06001872 RID: 6258 RVA: 0x0007F80D File Offset: 0x0007DA0D
		public IReadOnlyDictionary<NetUserId, HumanoidCharacterProfile> Profiles { get; }

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06001873 RID: 6259 RVA: 0x0007F815 File Offset: 0x0007DA15
		public bool Forced { get; }

		// Token: 0x06001874 RID: 6260 RVA: 0x0007F81D File Offset: 0x0007DA1D
		public RulePlayerSpawningEvent(List<IPlayerSession> playerPool, IReadOnlyDictionary<NetUserId, HumanoidCharacterProfile> profiles, bool forced)
		{
			this.PlayerPool = playerPool;
			this.Profiles = profiles;
			this.Forced = forced;
		}
	}
}
