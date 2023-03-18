using System;
using System.Runtime.CompilerServices;
using Content.Server.Radiation.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Radiation.Components
{
	// Token: 0x02000267 RID: 615
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RadiationSystem)
	})]
	public sealed class RadiationBlockerComponent : Component
	{
		// Token: 0x0400079B RID: 1947
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;

		// Token: 0x0400079C RID: 1948
		[DataField("resistance", false, 1, false, false, null)]
		public float RadResistance = 1f;

		// Token: 0x0400079D RID: 1949
		[TupleElementNames(new string[]
		{
			"Grid",
			"Tile"
		})]
		public ValueTuple<EntityUid, Vector2i>? CurrentPosition;
	}
}
