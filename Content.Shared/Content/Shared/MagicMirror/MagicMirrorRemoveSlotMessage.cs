using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror
{
	// Token: 0x02000343 RID: 835
	[NetSerializable]
	[Serializable]
	public sealed class MagicMirrorRemoveSlotMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060009D0 RID: 2512 RVA: 0x000205AC File Offset: 0x0001E7AC
		public MagicMirrorRemoveSlotMessage(MagicMirrorCategory category, int slot)
		{
			this.Category = category;
			this.Slot = slot;
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x060009D1 RID: 2513 RVA: 0x000205C2 File Offset: 0x0001E7C2
		public MagicMirrorCategory Category { get; }

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x060009D2 RID: 2514 RVA: 0x000205CA File Offset: 0x0001E7CA
		public int Slot { get; }
	}
}
