using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Lathe
{
	// Token: 0x0200037B RID: 891
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class LatheUpdateState : BoundUserInterfaceState
	{
		// Token: 0x06000A65 RID: 2661 RVA: 0x00022525 File Offset: 0x00020725
		public LatheUpdateState(List<string> recipes, List<LatheRecipePrototype> queue, [Nullable(2)] LatheRecipePrototype currentlyProducing = null)
		{
			this.Recipes = recipes;
			this.Queue = queue;
			this.CurrentlyProducing = currentlyProducing;
		}

		// Token: 0x04000A4D RID: 2637
		public List<string> Recipes;

		// Token: 0x04000A4E RID: 2638
		public List<LatheRecipePrototype> Queue;

		// Token: 0x04000A4F RID: 2639
		[Nullable(2)]
		public LatheRecipePrototype CurrentlyProducing;
	}
}
