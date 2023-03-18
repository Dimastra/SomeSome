using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Research.Components
{
	// Token: 0x0200020C RID: 524
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ResearchServerState : ComponentState
	{
		// Token: 0x060005D0 RID: 1488 RVA: 0x00014C8C File Offset: 0x00012E8C
		public ResearchServerState(string serverName, int points, int id)
		{
			this.ServerName = serverName;
			this.Points = points;
			this.Id = id;
		}

		// Token: 0x040005EC RID: 1516
		public string ServerName;

		// Token: 0x040005ED RID: 1517
		public int Points;

		// Token: 0x040005EE RID: 1518
		public int Id;
	}
}
