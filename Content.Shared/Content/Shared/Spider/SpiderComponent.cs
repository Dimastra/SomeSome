using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Spider
{
	// Token: 0x02000176 RID: 374
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedSpiderSystem)
	})]
	public sealed class SpiderComponent : Component
	{
		// Token: 0x04000440 RID: 1088
		[ViewVariables]
		[DataField("webPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string WebPrototype = "SpiderWeb";

		// Token: 0x04000441 RID: 1089
		[ViewVariables]
		[DataField("webActionName", false, 1, false, false, null)]
		public string WebActionName = "SpiderWebAction";
	}
}
