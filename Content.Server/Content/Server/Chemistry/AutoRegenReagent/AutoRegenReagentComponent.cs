using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.AutoRegenReagent
{
	// Token: 0x0200064B RID: 1611
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AutoRegenReagentComponent : Component
	{
		// Token: 0x0400151B RID: 5403
		[Nullable(2)]
		[DataField("solution", false, 1, true, false, null)]
		public string SolutionName;

		// Token: 0x0400151C RID: 5404
		[DataField("reagents", false, 1, true, false, null)]
		public List<string> Reagents;

		// Token: 0x0400151D RID: 5405
		public string CurrentReagent = "";

		// Token: 0x0400151E RID: 5406
		public int CurrentIndex;

		// Token: 0x0400151F RID: 5407
		[Nullable(2)]
		public Solution Solution;

		// Token: 0x04001520 RID: 5408
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;

		// Token: 0x04001521 RID: 5409
		[DataField("unitsPerSecond", false, 1, false, false, null)]
		public float unitsPerSecond = 0.2f;
	}
}
