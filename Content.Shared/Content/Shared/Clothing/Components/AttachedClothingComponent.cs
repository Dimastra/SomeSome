using System;
using Content.Shared.Clothing.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Clothing.Components
{
	// Token: 0x020005AF RID: 1455
	[Access(new Type[]
	{
		typeof(ToggleableClothingSystem)
	})]
	[RegisterComponent]
	public sealed class AttachedClothingComponent : Component
	{
		// Token: 0x04001064 RID: 4196
		[DataField("AttachedUid", false, 1, false, false, null)]
		public EntityUid AttachedUid;
	}
}
