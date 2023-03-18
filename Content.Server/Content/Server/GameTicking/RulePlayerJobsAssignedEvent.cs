using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Preferences;
using Robust.Server.Player;
using Robust.Shared.Network;

namespace Content.Server.GameTicking
{
	// Token: 0x020004B2 RID: 1202
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RulePlayerJobsAssignedEvent
	{
		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06001875 RID: 6261 RVA: 0x0007F83A File Offset: 0x0007DA3A
		public IPlayerSession[] Players { get; }

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06001876 RID: 6262 RVA: 0x0007F842 File Offset: 0x0007DA42
		public IReadOnlyDictionary<NetUserId, HumanoidCharacterProfile> Profiles { get; }

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06001877 RID: 6263 RVA: 0x0007F84A File Offset: 0x0007DA4A
		public bool Forced { get; }

		// Token: 0x06001878 RID: 6264 RVA: 0x0007F852 File Offset: 0x0007DA52
		public RulePlayerJobsAssignedEvent(IPlayerSession[] players, IReadOnlyDictionary<NetUserId, HumanoidCharacterProfile> profiles, bool forced)
		{
			this.Players = players;
			this.Profiles = profiles;
			this.Forced = forced;
		}
	}
}
