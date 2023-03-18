using System;
using Content.Server.Station.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Server.Station.Components
{
	// Token: 0x020001A5 RID: 421
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(StationSpawningSystem)
	})]
	public sealed class StationSpawningComponent : Component
	{
	}
}
