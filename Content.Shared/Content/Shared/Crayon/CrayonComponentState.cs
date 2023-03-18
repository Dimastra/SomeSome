using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Crayon
{
	// Token: 0x02000556 RID: 1366
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CrayonComponentState : ComponentState
	{
		// Token: 0x06001098 RID: 4248 RVA: 0x00036269 File Offset: 0x00034469
		public CrayonComponentState(Color color, string state, int charges, int capacity)
		{
			this.Color = color;
			this.State = state;
			this.Charges = charges;
			this.Capacity = capacity;
		}

		// Token: 0x04000F8F RID: 3983
		public readonly Color Color;

		// Token: 0x04000F90 RID: 3984
		public readonly string State;

		// Token: 0x04000F91 RID: 3985
		public readonly int Charges;

		// Token: 0x04000F92 RID: 3986
		public readonly int Capacity;
	}
}
