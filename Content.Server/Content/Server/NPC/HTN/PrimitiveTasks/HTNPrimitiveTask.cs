using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NPC.HTN.Preconditions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks
{
	// Token: 0x0200034D RID: 845
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("htnPrimitive", 1)]
	public sealed class HTNPrimitiveTask : HTNTask
	{
		// Token: 0x04000A9D RID: 2717
		[DataField("applyEffectsOnStartup", false, 1, false, false, null)]
		public bool ApplyEffectsOnStartup = true;

		// Token: 0x04000A9E RID: 2718
		[DataField("preconditions", false, 1, false, false, null)]
		public List<HTNPrecondition> Preconditions = new List<HTNPrecondition>();

		// Token: 0x04000A9F RID: 2719
		[DataField("operator", false, 1, true, false, null)]
		public HTNOperator Operator;
	}
}
