using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Gatherable.Components
{
	// Token: 0x020004A2 RID: 1186
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(GatherableSystem)
	})]
	public sealed class GatherableComponent : Component
	{
		// Token: 0x04000ECB RID: 3787
		[Nullable(2)]
		[DataField("whitelist", false, 1, true, false, null)]
		public EntityWhitelist ToolWhitelist;

		// Token: 0x04000ECC RID: 3788
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[DataField("loot", false, 1, false, false, null)]
		public Dictionary<string, string> MappedLoot = new Dictionary<string, string>();
	}
}
