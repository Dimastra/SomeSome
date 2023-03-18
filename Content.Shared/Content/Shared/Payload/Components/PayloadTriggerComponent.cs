using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Payload.Components
{
	// Token: 0x02000299 RID: 665
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class PayloadTriggerComponent : Component
	{
		// Token: 0x0400078E RID: 1934
		public bool Active;

		// Token: 0x0400078F RID: 1935
		[Nullable(2)]
		[DataField("components", true, 1, false, true, null)]
		public readonly EntityPrototype.ComponentRegistry Components;

		// Token: 0x04000790 RID: 1936
		[Nullable(1)]
		[DataField("grantedComponents", false, 1, false, true, null)]
		public readonly HashSet<Type> GrantedComponents = new HashSet<Type>();
	}
}
