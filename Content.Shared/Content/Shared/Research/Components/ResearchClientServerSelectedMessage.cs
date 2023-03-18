using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Research.Components
{
	// Token: 0x02000207 RID: 519
	[NetSerializable]
	[Serializable]
	public sealed class ResearchClientServerSelectedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060005CC RID: 1484 RVA: 0x00014C13 File Offset: 0x00012E13
		public ResearchClientServerSelectedMessage(int serverId)
		{
			this.ServerId = serverId;
		}

		// Token: 0x040005DF RID: 1503
		public int ServerId;
	}
}
