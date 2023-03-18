using System;
using System.Runtime.CompilerServices;
using Content.Server.Roles;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects;

namespace Content.Server.Station.Systems
{
	// Token: 0x0200019A RID: 410
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class PlayerSpawningEvent : EntityEventArgs
	{
		// Token: 0x06000832 RID: 2098 RVA: 0x00029AA0 File Offset: 0x00027CA0
		public PlayerSpawningEvent(Job job, HumanoidCharacterProfile humanoidCharacterProfile, EntityUid? station)
		{
			this.Job = job;
			this.HumanoidCharacterProfile = humanoidCharacterProfile;
			this.Station = station;
		}

		// Token: 0x040004FE RID: 1278
		public EntityUid? SpawnResult;

		// Token: 0x040004FF RID: 1279
		public readonly Job Job;

		// Token: 0x04000500 RID: 1280
		public readonly HumanoidCharacterProfile HumanoidCharacterProfile;

		// Token: 0x04000501 RID: 1281
		public readonly EntityUid? Station;
	}
}
