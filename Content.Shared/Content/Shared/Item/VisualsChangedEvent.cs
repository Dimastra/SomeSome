using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Item
{
	// Token: 0x020003A2 RID: 930
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class VisualsChangedEvent : EntityEventArgs
	{
		// Token: 0x06000AA9 RID: 2729 RVA: 0x00022C6F File Offset: 0x00020E6F
		public VisualsChangedEvent(EntityUid item, string containerId)
		{
			this.Item = item;
			this.ContainerId = containerId;
		}

		// Token: 0x04000AA0 RID: 2720
		public readonly EntityUid Item;

		// Token: 0x04000AA1 RID: 2721
		public readonly string ContainerId;
	}
}
