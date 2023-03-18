using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Components
{
	// Token: 0x02000547 RID: 1351
	[RegisterComponent]
	public sealed class StaminaDamageOnHitComponent : Component
	{
		// Token: 0x04000F78 RID: 3960
		[ViewVariables]
		[DataField("damage", false, 1, false, false, null)]
		public float Damage = 30f;
	}
}
