using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;

namespace Content.Shared.Players.PlayTimeTracking
{
	// Token: 0x0200026E RID: 622
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("playTimeTracker", 1)]
	public sealed class PlayTimeTrackerPrototype : IPrototype
	{
		// Token: 0x17000165 RID: 357
		// (get) Token: 0x0600071D RID: 1821 RVA: 0x000186AF File Offset: 0x000168AF
		[IdDataField(1, null)]
		public string ID { get; }
	}
}
