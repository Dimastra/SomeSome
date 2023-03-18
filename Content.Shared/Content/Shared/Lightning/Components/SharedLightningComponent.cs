using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Lightning.Components
{
	// Token: 0x02000360 RID: 864
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedLightningComponent : Component
	{
		// Token: 0x040009DA RID: 2522
		[ViewVariables]
		[DataField("canArc", false, 1, false, false, null)]
		public bool CanArc;

		// Token: 0x040009DB RID: 2523
		[ViewVariables]
		[DataField("maxTotalArc", false, 1, false, false, null)]
		public int MaxTotalArcs = 50;

		// Token: 0x040009DC RID: 2524
		[ViewVariables]
		[DataField("lightningPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string LightningPrototype = "Lightning";

		// Token: 0x040009DD RID: 2525
		[DataField("arcTarget", false, 1, false, false, null)]
		public EntityUid ArcTarget;

		// Token: 0x040009DE RID: 2526
		[ViewVariables]
		[DataField("maxLength", false, 1, false, false, null)]
		public float MaxLength = 5f;

		// Token: 0x040009DF RID: 2527
		[DataField("arcTargets", false, 1, false, false, null)]
		public HashSet<EntityUid> ArcTargets = new HashSet<EntityUid>();

		// Token: 0x040009E0 RID: 2528
		[DataField("collisionMask", false, 1, false, false, null)]
		public int CollisionMask = 30;
	}
}
