using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x0200069B RID: 1691
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SolutionChangedEvent : EntityEventArgs
	{
		// Token: 0x0600234F RID: 9039 RVA: 0x000B8614 File Offset: 0x000B6814
		public SolutionChangedEvent(Solution solution)
		{
			this.Solution = solution;
		}

		// Token: 0x040015BC RID: 5564
		public readonly Solution Solution;
	}
}
