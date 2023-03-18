using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x0200005D RID: 93
	[RegisterComponent]
	public sealed class SpawnArtifactComponent : Component
	{
		// Token: 0x040000DA RID: 218
		[Nullable(2)]
		[DataField("spawns", false, 1, false, false, null)]
		public List<EntitySpawnEntry> Spawns;

		// Token: 0x040000DB RID: 219
		[DataField("range", false, 1, false, false, null)]
		public float Range = 0.5f;

		// Token: 0x040000DC RID: 220
		[DataField("maxSpawns", false, 1, false, false, null)]
		public int MaxSpawns = 10;
	}
}
