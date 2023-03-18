using System;
using System.Runtime.CompilerServices;
using Content.Shared.Kitchen;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Kitchen.Components
{
	// Token: 0x02000432 RID: 1074
	[RegisterComponent]
	public sealed class ActiveMicrowaveComponent : Component
	{
		// Token: 0x04000D98 RID: 3480
		[ViewVariables]
		public float CookTimeRemaining;

		// Token: 0x04000D99 RID: 3481
		[ViewVariables]
		public float TotalTime;

		// Token: 0x04000D9A RID: 3482
		[Nullable(new byte[]
		{
			0,
			2
		})]
		[ViewVariables]
		public ValueTuple<FoodRecipePrototype, int> PortionedRecipe;
	}
}
