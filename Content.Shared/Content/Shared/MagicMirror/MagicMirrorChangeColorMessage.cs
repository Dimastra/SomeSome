using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror
{
	// Token: 0x02000342 RID: 834
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class MagicMirrorChangeColorMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060009CC RID: 2508 RVA: 0x00020577 File Offset: 0x0001E777
		public MagicMirrorChangeColorMessage(MagicMirrorCategory category, List<Color> colors, int slot)
		{
			this.Category = category;
			this.Colors = colors;
			this.Slot = slot;
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x060009CD RID: 2509 RVA: 0x00020594 File Offset: 0x0001E794
		public MagicMirrorCategory Category { get; }

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x060009CE RID: 2510 RVA: 0x0002059C File Offset: 0x0001E79C
		public List<Color> Colors { get; }

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x060009CF RID: 2511 RVA: 0x000205A4 File Offset: 0x0001E7A4
		public int Slot { get; }
	}
}
