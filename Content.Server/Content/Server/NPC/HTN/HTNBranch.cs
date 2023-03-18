using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NPC.HTN.Preconditions;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN
{
	// Token: 0x02000343 RID: 835
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class HTNBranch
	{
		// Token: 0x04000A7F RID: 2687
		[DataField("preconditions", false, 1, false, false, null)]
		public List<HTNPrecondition> Preconditions = new List<HTNPrecondition>();

		// Token: 0x04000A80 RID: 2688
		[DataField("tasks", false, 1, true, false, typeof(HTNTaskListSerializer))]
		public List<string> TaskPrototypes;
	}
}
