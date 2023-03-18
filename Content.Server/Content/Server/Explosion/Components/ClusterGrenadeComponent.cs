using System;
using System.Runtime.CompilerServices;
using Content.Server.Explosion.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Explosion.Components
{
	// Token: 0x02000515 RID: 1301
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ClusterGrenadeSystem)
	})]
	public sealed class ClusterGrenadeComponent : Component
	{
		// Token: 0x04001163 RID: 4451
		[Nullable(1)]
		public Container GrenadesContainer;

		// Token: 0x04001164 RID: 4452
		[Nullable(2)]
		[DataField("fillPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string FillPrototype;

		// Token: 0x04001165 RID: 4453
		public int UnspawnedCount;

		// Token: 0x04001166 RID: 4454
		[DataField("maxGrenadesCount", false, 1, false, false, null)]
		public int MaxGrenades = 3;

		// Token: 0x04001167 RID: 4455
		[ViewVariables]
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 1f;

		// Token: 0x04001168 RID: 4456
		[ViewVariables]
		[DataField("distance", false, 1, false, false, null)]
		public float ThrowDistance = 50f;

		// Token: 0x04001169 RID: 4457
		public bool CountDown;
	}
}
