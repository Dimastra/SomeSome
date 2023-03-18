using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Item
{
	// Token: 0x020003A4 RID: 932
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MultiHandedItemComponent : Component
	{
		// Token: 0x04000AAA RID: 2730
		[DataField("handsNeeded", false, 1, false, false, null)]
		[ViewVariables]
		public int HandsNeeded = 2;
	}
}
