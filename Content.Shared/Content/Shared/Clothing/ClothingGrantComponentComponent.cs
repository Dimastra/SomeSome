using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Clothing
{
	// Token: 0x020005A8 RID: 1448
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ClothingGrantComponentComponent : Component
	{
		// Token: 0x1700038A RID: 906
		// (get) Token: 0x060011AA RID: 4522 RVA: 0x00039C59 File Offset: 0x00037E59
		[DataField("component", false, 1, true, false, null)]
		[AlwaysPushInheritance]
		public EntityPrototype.ComponentRegistry Components { get; } = new EntityPrototype.ComponentRegistry();

		// Token: 0x04001051 RID: 4177
		[ViewVariables]
		public bool IsActive;
	}
}
