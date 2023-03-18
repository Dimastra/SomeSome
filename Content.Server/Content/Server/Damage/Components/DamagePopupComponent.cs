using System;
using Content.Server.Damage.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Damage.Components
{
	// Token: 0x020005CD RID: 1485
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(DamagePopupSystem)
	})]
	public sealed class DamagePopupComponent : Component
	{
		// Token: 0x040013A9 RID: 5033
		[DataField("damagePopupType", false, 1, false, false, null)]
		[ViewVariables]
		public DamagePopupType Type;
	}
}
