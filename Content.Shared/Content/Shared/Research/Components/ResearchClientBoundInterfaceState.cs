using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Research.Components
{
	// Token: 0x0200020A RID: 522
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ResearchClientBoundInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x060005CE RID: 1486 RVA: 0x00014C2A File Offset: 0x00012E2A
		public ResearchClientBoundInterfaceState(int serverCount, string[] serverNames, int[] serverIds, int selectedServerId = -1)
		{
			this.ServerCount = serverCount;
			this.ServerNames = serverNames;
			this.ServerIds = serverIds;
			this.SelectedServerId = selectedServerId;
		}

		// Token: 0x040005E2 RID: 1506
		public int ServerCount;

		// Token: 0x040005E3 RID: 1507
		public string[] ServerNames;

		// Token: 0x040005E4 RID: 1508
		public int[] ServerIds;

		// Token: 0x040005E5 RID: 1509
		public int SelectedServerId;
	}
}
