using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Crayon
{
	// Token: 0x02000554 RID: 1364
	[NetSerializable]
	[Serializable]
	public sealed class CrayonColorMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001097 RID: 4247 RVA: 0x0003625A File Offset: 0x0003445A
		public CrayonColorMessage(Color color)
		{
			this.Color = color;
		}

		// Token: 0x04000F8B RID: 3979
		public readonly Color Color;
	}
}
