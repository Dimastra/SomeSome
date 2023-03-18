using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Disposal
{
	// Token: 0x020004FC RID: 1276
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class TargetSelectedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000F74 RID: 3956 RVA: 0x00032440 File Offset: 0x00030640
		public TargetSelectedMessage(string target)
		{
			this.target = target;
		}

		// Token: 0x04000EBD RID: 3773
		public readonly string target;
	}
}
