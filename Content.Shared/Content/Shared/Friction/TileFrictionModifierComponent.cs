using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Friction
{
	// Token: 0x0200046F RID: 1135
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(TileFrictionController)
	})]
	public sealed class TileFrictionModifierComponent : Component
	{
		// Token: 0x04000D1E RID: 3358
		[ViewVariables]
		[DataField("modifier", false, 1, false, false, null)]
		public float Modifier;
	}
}
