using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Clothing
{
	// Token: 0x020005A3 RID: 1443
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(ClothingSpeedModifierSystem)
	})]
	public sealed class ClothingSpeedModifierComponent : Component
	{
		// Token: 0x0400103E RID: 4158
		[DataField("walkModifier", false, 1, true, false, null)]
		[ViewVariables]
		public float WalkModifier = 1f;

		// Token: 0x0400103F RID: 4159
		[DataField("sprintModifier", false, 1, true, false, null)]
		[ViewVariables]
		public float SprintModifier = 1f;

		// Token: 0x04001040 RID: 4160
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;
	}
}
