using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Clothing
{
	// Token: 0x020005A4 RID: 1444
	[NetSerializable]
	[Serializable]
	public sealed class ClothingSpeedModifierComponentState : ComponentState
	{
		// Token: 0x06001199 RID: 4505 RVA: 0x00039613 File Offset: 0x00037813
		public ClothingSpeedModifierComponentState(float walkModifier, float sprintModifier, bool enabled)
		{
			this.WalkModifier = walkModifier;
			this.SprintModifier = sprintModifier;
			this.Enabled = enabled;
		}

		// Token: 0x04001041 RID: 4161
		public float WalkModifier;

		// Token: 0x04001042 RID: 4162
		public float SprintModifier;

		// Token: 0x04001043 RID: 4163
		public bool Enabled;
	}
}
