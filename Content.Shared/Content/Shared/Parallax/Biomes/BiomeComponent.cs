using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Parallax.Biomes
{
	// Token: 0x0200029C RID: 668
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class BiomeComponent : Component
	{
		// Token: 0x04000792 RID: 1938
		[ViewVariables]
		[DataField("seed", false, 1, false, false, null)]
		public int Seed;

		// Token: 0x04000793 RID: 1939
		[ViewVariables]
		[DataField("prototype", false, 1, false, false, typeof(PrototypeIdSerializer<BiomePrototype>))]
		public string BiomePrototype = "Grasslands";

		// Token: 0x04000794 RID: 1940
		[DataField("modifiedTiles", false, 1, false, false, null)]
		public Dictionary<Vector2i, HashSet<Vector2i>> ModifiedTiles = new Dictionary<Vector2i, HashSet<Vector2i>>();

		// Token: 0x04000795 RID: 1941
		[DataField("decals", false, 1, false, false, null)]
		public Dictionary<Vector2i, Dictionary<uint, Vector2i>> LoadedDecals = new Dictionary<Vector2i, Dictionary<uint, Vector2i>>();

		// Token: 0x04000796 RID: 1942
		[DataField("entities", false, 1, false, false, null)]
		public Dictionary<Vector2i, List<EntityUid>> LoadedEntities = new Dictionary<Vector2i, List<EntityUid>>();

		// Token: 0x04000797 RID: 1943
		[DataField("loadedChunks", false, 1, false, false, null)]
		public readonly HashSet<Vector2i> LoadedChunks = new HashSet<Vector2i>();
	}
}
