using System;
using System.Runtime.CompilerServices;
using Content.Shared.Preferences;
using Robust.Server.Player;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking
{
	// Token: 0x020004B5 RID: 1205
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PlayerSpawnCompleteEvent : EntityEventArgs
	{
		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06001883 RID: 6275 RVA: 0x0007F921 File Offset: 0x0007DB21
		public EntityUid Mob { get; }

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06001884 RID: 6276 RVA: 0x0007F929 File Offset: 0x0007DB29
		public IPlayerSession Player { get; }

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06001885 RID: 6277 RVA: 0x0007F931 File Offset: 0x0007DB31
		[Nullable(2)]
		public string JobId { [NullableContext(2)] get; }

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06001886 RID: 6278 RVA: 0x0007F939 File Offset: 0x0007DB39
		public bool LateJoin { get; }

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06001887 RID: 6279 RVA: 0x0007F941 File Offset: 0x0007DB41
		public EntityUid Station { get; }

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06001888 RID: 6280 RVA: 0x0007F949 File Offset: 0x0007DB49
		public HumanoidCharacterProfile Profile { get; }

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06001889 RID: 6281 RVA: 0x0007F951 File Offset: 0x0007DB51
		public int JoinOrder { get; }

		// Token: 0x0600188A RID: 6282 RVA: 0x0007F959 File Offset: 0x0007DB59
		public PlayerSpawnCompleteEvent(EntityUid mob, IPlayerSession player, [Nullable(2)] string jobId, bool lateJoin, int joinOrder, EntityUid station, HumanoidCharacterProfile profile)
		{
			this.Mob = mob;
			this.Player = player;
			this.JobId = jobId;
			this.LateJoin = lateJoin;
			this.Station = station;
			this.Profile = profile;
			this.JoinOrder = joinOrder;
		}
	}
}
