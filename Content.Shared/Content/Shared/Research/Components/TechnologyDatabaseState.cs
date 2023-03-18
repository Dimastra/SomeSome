using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Research.Components
{
	// Token: 0x02000216 RID: 534
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class TechnologyDatabaseState : ComponentState
	{
		// Token: 0x060005F9 RID: 1529 RVA: 0x000150FB File Offset: 0x000132FB
		public TechnologyDatabaseState(List<string> technologies, List<string> recipes)
		{
			this.Technologies = technologies;
			this.Recipes = recipes;
		}

		// Token: 0x040005FB RID: 1531
		public List<string> Technologies;

		// Token: 0x040005FC RID: 1532
		public List<string> Recipes;
	}
}
