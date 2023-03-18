using System;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Singularity.Events
{
	// Token: 0x0200019B RID: 411
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SingularityLevelChangedEvent : EntityEventArgs
	{
		// Token: 0x060004D6 RID: 1238 RVA: 0x00012961 File Offset: 0x00010B61
		public SingularityLevelChangedEvent(byte newValue, byte oldValue, SingularityComponent singularity)
		{
			this.NewValue = newValue;
			this.OldValue = oldValue;
			this.Singularity = singularity;
		}

		// Token: 0x0400047E RID: 1150
		public readonly byte NewValue;

		// Token: 0x0400047F RID: 1151
		public readonly byte OldValue;

		// Token: 0x04000480 RID: 1152
		public readonly SingularityComponent Singularity;
	}
}
