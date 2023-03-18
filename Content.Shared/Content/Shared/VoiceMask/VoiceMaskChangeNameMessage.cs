using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.VoiceMask
{
	// Token: 0x02000084 RID: 132
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class VoiceMaskChangeNameMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600018D RID: 397 RVA: 0x00008E13 File Offset: 0x00007013
		public string Name { get; }

		// Token: 0x0600018E RID: 398 RVA: 0x00008E1B File Offset: 0x0000701B
		public VoiceMaskChangeNameMessage(string name)
		{
			this.Name = name;
		}
	}
}
