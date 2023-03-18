using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror
{
	// Token: 0x02000345 RID: 837
	[NetSerializable]
	[Serializable]
	public sealed class MagicMirrorAddSlotMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060009D6 RID: 2518 RVA: 0x000205F8 File Offset: 0x0001E7F8
		public MagicMirrorAddSlotMessage(MagicMirrorCategory category)
		{
			this.Category = category;
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x060009D7 RID: 2519 RVA: 0x00020607 File Offset: 0x0001E807
		public MagicMirrorCategory Category { get; }
	}
}
