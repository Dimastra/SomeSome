using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Maps
{
	// Token: 0x020003D2 RID: 978
	[ImplicitDataDefinitionForInheritors]
	public abstract class GameMapCondition
	{
		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06001409 RID: 5129 RVA: 0x00068455 File Offset: 0x00066655
		[DataField("inverted", false, 1, false, false, null)]
		public bool Inverted { get; }

		// Token: 0x0600140A RID: 5130
		[NullableContext(1)]
		public abstract bool Check(GameMapPrototype map);
	}
}
