using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC
{
	// Token: 0x0200032E RID: 814
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FactionData
	{
		// Token: 0x040009F6 RID: 2550
		[ViewVariables]
		public HashSet<string> Friendly = new HashSet<string>();

		// Token: 0x040009F7 RID: 2551
		[ViewVariables]
		public HashSet<string> Hostile = new HashSet<string>();
	}
}
