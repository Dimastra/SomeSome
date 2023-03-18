using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Station.Components
{
	// Token: 0x020001A0 RID: 416
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(GameTicker)
	})]
	public sealed class BecomesStationComponent : Component
	{
		// Token: 0x04000515 RID: 1301
		[Nullable(1)]
		[DataField("id", false, 1, true, false, null)]
		[ViewVariables]
		public string Id;
	}
}
