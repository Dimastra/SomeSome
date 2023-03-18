using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.CartridgeLoader.Cartridges
{
	// Token: 0x020006DE RID: 1758
	[RegisterComponent]
	public sealed class NotekeeperCartridgeComponent : Component
	{
		// Token: 0x0400168F RID: 5775
		[Nullable(1)]
		[DataField("notes", false, 1, false, false, null)]
		public List<string> Notes = new List<string>();
	}
}
