using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005F5 RID: 1525
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReactionAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x0600128B RID: 4747 RVA: 0x0003CB54 File Offset: 0x0003AD54
		public ReactionAttemptEvent(ReactionPrototype reaction, Solution solution)
		{
			this.Reaction = reaction;
			this.Solution = solution;
		}

		// Token: 0x0400114C RID: 4428
		public readonly ReactionPrototype Reaction;

		// Token: 0x0400114D RID: 4429
		public readonly Solution Solution;
	}
}
