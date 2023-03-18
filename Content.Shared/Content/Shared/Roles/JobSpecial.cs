using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Roles
{
	// Token: 0x020001EA RID: 490
	[ImplicitDataDefinitionForInheritors]
	public abstract class JobSpecial
	{
		// Token: 0x06000583 RID: 1411
		public abstract void AfterEquip(EntityUid mob);
	}
}
