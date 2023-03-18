using System;
using System.Runtime.CompilerServices;
using Content.Shared.Preferences;
using Robust.Server.Player;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking
{
	// Token: 0x020004B4 RID: 1204
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PlayerBeforeSpawnEvent : HandledEntityEventArgs
	{
		// Token: 0x1700036C RID: 876
		// (get) Token: 0x0600187D RID: 6269 RVA: 0x0007F8CC File Offset: 0x0007DACC
		public IPlayerSession Player { get; }

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x0600187E RID: 6270 RVA: 0x0007F8D4 File Offset: 0x0007DAD4
		public HumanoidCharacterProfile Profile { get; }

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x0600187F RID: 6271 RVA: 0x0007F8DC File Offset: 0x0007DADC
		[Nullable(2)]
		public string JobId { [NullableContext(2)] get; }

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06001880 RID: 6272 RVA: 0x0007F8E4 File Offset: 0x0007DAE4
		public bool LateJoin { get; }

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06001881 RID: 6273 RVA: 0x0007F8EC File Offset: 0x0007DAEC
		public EntityUid Station { get; }

		// Token: 0x06001882 RID: 6274 RVA: 0x0007F8F4 File Offset: 0x0007DAF4
		public PlayerBeforeSpawnEvent(IPlayerSession player, HumanoidCharacterProfile profile, [Nullable(2)] string jobId, bool lateJoin, EntityUid station)
		{
			this.Player = player;
			this.Profile = profile;
			this.JobId = jobId;
			this.LateJoin = lateJoin;
			this.Station = station;
		}
	}
}
