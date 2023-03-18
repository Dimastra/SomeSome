using System;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Labels.Components
{
	// Token: 0x02000429 RID: 1065
	[RegisterComponent]
	public sealed class PaperLabelComponent : Component
	{
		// Token: 0x04000D67 RID: 3431
		[Nullable(1)]
		[DataField("labelSlot", false, 1, false, false, null)]
		public ItemSlot LabelSlot = new ItemSlot();
	}
}
