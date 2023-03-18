using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Wires
{
	// Token: 0x02000077 RID: 119
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TimedWireEvent : EntityEventArgs
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x00009DDF File Offset: 0x00007FDF
		public WireActionDelegate Delegate { get; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x00009DE7 File Offset: 0x00007FE7
		public Wire Wire { get; }

		// Token: 0x060001B9 RID: 441 RVA: 0x00009DEF File Offset: 0x00007FEF
		public TimedWireEvent(WireActionDelegate @delegate, Wire wire)
		{
			this.Delegate = @delegate;
			this.Wire = wire;
		}
	}
}
