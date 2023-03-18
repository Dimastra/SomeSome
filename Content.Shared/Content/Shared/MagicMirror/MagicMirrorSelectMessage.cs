using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror
{
	// Token: 0x02000341 RID: 833
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class MagicMirrorSelectMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060009C8 RID: 2504 RVA: 0x00020542 File Offset: 0x0001E742
		public MagicMirrorSelectMessage(MagicMirrorCategory category, string marking, int slot)
		{
			this.Category = category;
			this.Marking = marking;
			this.Slot = slot;
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x060009C9 RID: 2505 RVA: 0x0002055F File Offset: 0x0001E75F
		public MagicMirrorCategory Category { get; }

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x060009CA RID: 2506 RVA: 0x00020567 File Offset: 0x0001E767
		public string Marking { get; }

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x060009CB RID: 2507 RVA: 0x0002056F File Offset: 0x0001E76F
		public int Slot { get; }
	}
}
