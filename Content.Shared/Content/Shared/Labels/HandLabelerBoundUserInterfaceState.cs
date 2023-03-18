using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Labels
{
	// Token: 0x02000384 RID: 900
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class HandLabelerBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06000A71 RID: 2673 RVA: 0x000226D4 File Offset: 0x000208D4
		public string CurrentLabel { get; }

		// Token: 0x06000A72 RID: 2674 RVA: 0x000226DC File Offset: 0x000208DC
		public HandLabelerBoundUserInterfaceState(string currentLabel)
		{
			this.CurrentLabel = currentLabel;
		}
	}
}
