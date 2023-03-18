using System;
using System.Runtime.CompilerServices;
using Content.Server.Singularity.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Singularity.Components
{
	// Token: 0x020001F2 RID: 498
	[RegisterComponent]
	public sealed class SingularityGeneratorComponent : Component
	{
		// Token: 0x040005CD RID: 1485
		[DataField("power", false, 1, false, false, null)]
		[Access(new Type[]
		{
			typeof(SingularityGeneratorSystem)
		})]
		public float Power;

		// Token: 0x040005CE RID: 1486
		[DataField("threshold", false, 1, false, false, null)]
		[Access(new Type[]
		{
			typeof(SingularityGeneratorSystem)
		})]
		public float Threshold = 16f;

		// Token: 0x040005CF RID: 1487
		[Nullable(2)]
		[DataField("spawnId", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		[ViewVariables]
		public string SpawnPrototype = "Singularity";
	}
}
