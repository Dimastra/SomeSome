using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chemistry.Reagent
{
	// Token: 0x020005E9 RID: 1513
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ReactiveReagentEffectEntry
	{
		// Token: 0x04001128 RID: 4392
		[DataField("methods", false, 1, true, false, null)]
		public HashSet<ReactionMethod> Methods;

		// Token: 0x04001129 RID: 4393
		[DataField("effects", false, 1, true, false, null)]
		public ReagentEffect[] Effects;
	}
}
