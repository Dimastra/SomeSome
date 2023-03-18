using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.NodeContainer
{
	// Token: 0x020002D5 RID: 725
	[NullableContext(1)]
	[Nullable(0)]
	public static class NodeVis
	{
		// Token: 0x020007C6 RID: 1990
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public sealed class MsgEnable : EntityEventArgs
		{
			// Token: 0x06001826 RID: 6182 RVA: 0x0004D8D7 File Offset: 0x0004BAD7
			public MsgEnable(bool enabled)
			{
				this.Enabled = enabled;
			}

			// Token: 0x170004FA RID: 1274
			// (get) Token: 0x06001827 RID: 6183 RVA: 0x0004D8E6 File Offset: 0x0004BAE6
			public bool Enabled { get; }
		}

		// Token: 0x020007C7 RID: 1991
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class MsgData : EntityEventArgs
		{
			// Token: 0x04001806 RID: 6150
			public List<NodeVis.GroupData> Groups = new List<NodeVis.GroupData>();

			// Token: 0x04001807 RID: 6151
			public List<int> GroupDeletions = new List<int>();

			// Token: 0x04001808 RID: 6152
			[Nullable(new byte[]
			{
				1,
				2
			})]
			public Dictionary<int, string> GroupDataUpdates = new Dictionary<int, string>();
		}

		// Token: 0x020007C8 RID: 1992
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class GroupData
		{
			// Token: 0x04001809 RID: 6153
			public int NetId;

			// Token: 0x0400180A RID: 6154
			public string GroupId = "";

			// Token: 0x0400180B RID: 6155
			public Color Color;

			// Token: 0x0400180C RID: 6156
			public NodeVis.NodeDatum[] Nodes = Array.Empty<NodeVis.NodeDatum>();

			// Token: 0x0400180D RID: 6157
			[Nullable(2)]
			public string DebugData;
		}

		// Token: 0x020007C9 RID: 1993
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class NodeDatum
		{
			// Token: 0x0400180E RID: 6158
			public EntityUid Entity;

			// Token: 0x0400180F RID: 6159
			public int NetId;

			// Token: 0x04001810 RID: 6160
			public int[] Reachable = Array.Empty<int>();

			// Token: 0x04001811 RID: 6161
			public string Name = "";

			// Token: 0x04001812 RID: 6162
			public string Type = "";
		}
	}
}
