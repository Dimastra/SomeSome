using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Lathe
{
	// Token: 0x0200037A RID: 890
	public sealed class LatheGetRecipesEvent : EntityEventArgs
	{
		// Token: 0x06000A64 RID: 2660 RVA: 0x0002250B File Offset: 0x0002070B
		public LatheGetRecipesEvent(EntityUid lathe)
		{
			this.Lathe = lathe;
		}

		// Token: 0x04000A4B RID: 2635
		public readonly EntityUid Lathe;

		// Token: 0x04000A4C RID: 2636
		[Nullable(1)]
		public List<string> Recipes = new List<string>();
	}
}
