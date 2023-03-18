using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror
{
	// Token: 0x02000344 RID: 836
	[NetSerializable]
	[Serializable]
	public sealed class MagicMirrorSelectSlotMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060009D3 RID: 2515 RVA: 0x000205D2 File Offset: 0x0001E7D2
		public MagicMirrorSelectSlotMessage(MagicMirrorCategory category, int slot)
		{
			this.Category = category;
			this.Slot = slot;
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x060009D4 RID: 2516 RVA: 0x000205E8 File Offset: 0x0001E7E8
		public MagicMirrorCategory Category { get; }

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x060009D5 RID: 2517 RVA: 0x000205F0 File Offset: 0x0001E7F0
		public int Slot { get; }
	}
}
