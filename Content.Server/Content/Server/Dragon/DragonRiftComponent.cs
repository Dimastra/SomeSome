using System;
using System.Runtime.CompilerServices;
using Content.Shared.Dragon;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Dragon
{
	// Token: 0x0200053D RID: 1341
	[RegisterComponent]
	public sealed class DragonRiftComponent : SharedDragonRiftComponent
	{
		// Token: 0x04001209 RID: 4617
		[DataField("dragon", false, 1, false, false, null)]
		public EntityUid Dragon;

		// Token: 0x0400120A RID: 4618
		[ViewVariables]
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;

		// Token: 0x0400120B RID: 4619
		[ViewVariables]
		[DataField("maxAccumuluator", false, 1, false, false, null)]
		public float MaxAccumulator = 300f;

		// Token: 0x0400120C RID: 4620
		[ViewVariables]
		[DataField("spawnAccumulator", false, 1, false, false, null)]
		public float SpawnAccumulator = 30f;

		// Token: 0x0400120D RID: 4621
		[ViewVariables]
		[DataField("spawnCooldown", false, 1, false, false, null)]
		public float SpawnCooldown = 30f;

		// Token: 0x0400120E RID: 4622
		[Nullable(1)]
		[ViewVariables]
		[DataField("spawn", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string SpawnPrototype = "MobCarpDragon";
	}
}
