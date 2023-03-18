using System;
using System.Runtime.CompilerServices;
using Content.Server.Botany.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Botany.Components
{
	// Token: 0x02000700 RID: 1792
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(LogSystem)
	})]
	public sealed class LogComponent : Component
	{
		// Token: 0x0400172D RID: 5933
		[Nullable(1)]
		[DataField("spawnedPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string SpawnedPrototype = "MaterialWoodPlank1";

		// Token: 0x0400172E RID: 5934
		[DataField("spawnCount", false, 1, false, false, null)]
		public int SpawnCount = 2;
	}
}
