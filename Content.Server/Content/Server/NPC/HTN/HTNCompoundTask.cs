using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN
{
	// Token: 0x02000345 RID: 837
	[Prototype("htnCompound", 1)]
	public sealed class HTNCompoundTask : HTNTask
	{
		// Token: 0x04000A87 RID: 2695
		[Nullable(1)]
		[DataField("branches", false, 1, true, false, null)]
		public List<HTNBranch> Branches;
	}
}
