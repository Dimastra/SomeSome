using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Tools.Components
{
	// Token: 0x020000C0 RID: 192
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class ToolEventData
	{
		// Token: 0x0600021D RID: 541 RVA: 0x0000AAA2 File Offset: 0x00008CA2
		public ToolEventData(object ev, float fuel = 0f, object cancelledEv = null, EntityUid? targetEntity = null)
		{
			this.Ev = ev;
			this.CancelledEv = cancelledEv;
			this.Fuel = fuel;
			this.TargetEntity = targetEntity;
		}

		// Token: 0x0400028F RID: 655
		public readonly object Ev;

		// Token: 0x04000290 RID: 656
		public readonly object CancelledEv;

		// Token: 0x04000291 RID: 657
		public readonly float Fuel;

		// Token: 0x04000292 RID: 658
		public readonly EntityUid? TargetEntity;
	}
}
