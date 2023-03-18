using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Components
{
	// Token: 0x020005F7 RID: 1527
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class FitsInDispenserComponent : Component
	{
		// Token: 0x170003BA RID: 954
		// (get) Token: 0x0600128F RID: 4751 RVA: 0x0003CB8D File Offset: 0x0003AD8D
		// (set) Token: 0x06001290 RID: 4752 RVA: 0x0003CB95 File Offset: 0x0003AD95
		[ViewVariables]
		[DataField("solution", false, 1, false, false, null)]
		public string Solution { get; set; } = "default";
	}
}
