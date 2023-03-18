using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Mech.Equipment.Components
{
	// Token: 0x02000324 RID: 804
	[RegisterComponent]
	public sealed class MechEquipmentComponent : Component
	{
		// Token: 0x04000918 RID: 2328
		[DataField("installDuration", false, 1, false, false, null)]
		public float InstallDuration = 5f;

		// Token: 0x04000919 RID: 2329
		[ViewVariables]
		public EntityUid? EquipmentOwner;
	}
}
