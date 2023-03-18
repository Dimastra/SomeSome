using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Explosion.Components
{
	// Token: 0x0200051A RID: 1306
	[RegisterComponent]
	public sealed class GibOnTriggerComponent : Component
	{
		// Token: 0x04001175 RID: 4469
		[ViewVariables]
		[DataField("deleteItems", false, 1, false, false, null)]
		public bool DeleteItems;
	}
}
