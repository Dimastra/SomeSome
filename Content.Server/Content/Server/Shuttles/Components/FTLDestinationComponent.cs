using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Shuttles.Components
{
	// Token: 0x02000206 RID: 518
	[RegisterComponent]
	public sealed class FTLDestinationComponent : Component
	{
		// Token: 0x0400064C RID: 1612
		[Nullable(2)]
		[ViewVariables]
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x0400064D RID: 1613
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;
	}
}
