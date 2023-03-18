using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Station.Components
{
	// Token: 0x020001A1 RID: 417
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(GameTicker)
	})]
	[Obsolete("Performs the exact same function as BecomesStationComponent.")]
	public sealed class PartOfStationComponent : Component
	{
		// Token: 0x04000516 RID: 1302
		[Nullable(1)]
		[DataField("id", false, 1, true, false, null)]
		[ViewVariables]
		public string Id;
	}
}
