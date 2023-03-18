using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Foldable
{
	// Token: 0x0200047D RID: 1149
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedFoldableSystem)
	})]
	public sealed class FoldableComponent : Component
	{
		// Token: 0x04000D2F RID: 3375
		[DataField("folded", false, 1, false, false, null)]
		public bool IsFolded;
	}
}
