using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x0200069F RID: 1695
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SolutionSpikeOverflowEvent : HandledEntityEventArgs
	{
		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x0600235E RID: 9054 RVA: 0x000B8AD5 File Offset: 0x000B6CD5
		public Solution Overflow { get; }

		// Token: 0x0600235F RID: 9055 RVA: 0x000B8ADD File Offset: 0x000B6CDD
		public SolutionSpikeOverflowEvent(Solution overflow)
		{
			this.Overflow = overflow;
		}
	}
}
