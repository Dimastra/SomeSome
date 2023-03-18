using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.GameTicking
{
	// Token: 0x02000463 RID: 1123
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedGameTicker : EntitySystem
	{
		// Token: 0x04000CFA RID: 3322
		public const string FallbackOverflowJob = "Passenger";

		// Token: 0x04000CFB RID: 3323
		public const string FallbackOverflowJobName = "job-name-passenger";
	}
}
