using System;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Body.Organ
{
	// Token: 0x0200065F RID: 1631
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedBodySystem)
	})]
	public sealed class OrganComponent : Component
	{
		// Token: 0x040013B1 RID: 5041
		[DataField("body", false, 1, false, false, null)]
		public EntityUid? Body;

		// Token: 0x040013B2 RID: 5042
		[Nullable(2)]
		[DataField("parent", false, 1, false, false, null)]
		public OrganSlot ParentSlot;
	}
}
