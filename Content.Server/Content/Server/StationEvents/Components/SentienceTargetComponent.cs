using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.StationEvents.Components
{
	// Token: 0x02000195 RID: 405
	[RegisterComponent]
	public sealed class SentienceTargetComponent : Component
	{
		// Token: 0x040004E2 RID: 1250
		[Nullable(1)]
		[DataField("flavorKind", false, 1, true, false, null)]
		public string FlavorKind;
	}
}
