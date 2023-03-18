using System;
using Content.Server.Station.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Station.Components
{
	// Token: 0x020001A4 RID: 420
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(StationSystem)
	})]
	public sealed class StationMemberComponent : Component
	{
		// Token: 0x04000521 RID: 1313
		[ViewVariables]
		public EntityUid Station = EntityUid.Invalid;
	}
}
