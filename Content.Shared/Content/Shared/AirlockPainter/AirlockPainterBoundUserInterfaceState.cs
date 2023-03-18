using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.AirlockPainter
{
	// Token: 0x02000728 RID: 1832
	[NetSerializable]
	[Serializable]
	public sealed class AirlockPainterBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06001635 RID: 5685 RVA: 0x00048B2B File Offset: 0x00046D2B
		public int SelectedStyle { get; }

		// Token: 0x06001636 RID: 5686 RVA: 0x00048B33 File Offset: 0x00046D33
		public AirlockPainterBoundUserInterfaceState(int selectedStyle)
		{
			this.SelectedStyle = selectedStyle;
		}
	}
}
