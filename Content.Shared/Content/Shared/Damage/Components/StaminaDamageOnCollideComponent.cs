using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Components
{
	// Token: 0x02000546 RID: 1350
	[RegisterComponent]
	public sealed class StaminaDamageOnCollideComponent : Component
	{
		// Token: 0x04000F77 RID: 3959
		[ViewVariables]
		[DataField("damage", false, 1, false, false, null)]
		public float Damage = 55f;
	}
}
