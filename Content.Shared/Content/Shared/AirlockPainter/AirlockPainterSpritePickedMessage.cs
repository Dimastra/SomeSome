using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.AirlockPainter
{
	// Token: 0x02000727 RID: 1831
	[NetSerializable]
	[Serializable]
	public sealed class AirlockPainterSpritePickedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x06001633 RID: 5683 RVA: 0x00048B14 File Offset: 0x00046D14
		public int Index { get; }

		// Token: 0x06001634 RID: 5684 RVA: 0x00048B1C File Offset: 0x00046D1C
		public AirlockPainterSpritePickedMessage(int index)
		{
			this.Index = index;
		}
	}
}
