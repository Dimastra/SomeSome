using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Anomaly.Effects.Components
{
	// Token: 0x02000708 RID: 1800
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class FleshAnomalyComponent : Component
	{
		// Token: 0x040015EA RID: 5610
		[DataField("spawns", false, 1, true, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		[ViewVariables]
		public List<string> Spawns = new List<string>();

		// Token: 0x040015EB RID: 5611
		[DataField("maxSpawnAmount", false, 1, false, false, null)]
		[ViewVariables]
		public int MaxSpawnAmount = 7;

		// Token: 0x040015EC RID: 5612
		[DataField("spawnRange", false, 1, false, false, null)]
		[ViewVariables]
		public float SpawnRange = 5f;

		// Token: 0x040015ED RID: 5613
		[DataField("fleshTileId", false, 1, false, false, typeof(PrototypeIdSerializer<ContentTileDefinition>))]
		[ViewVariables]
		public string FleshTileId = "FloorFlesh";

		// Token: 0x040015EE RID: 5614
		[DataField("superCriticalSpawn", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		[ViewVariables]
		public string SupercriticalSpawn = "FleshKudzu";
	}
}
