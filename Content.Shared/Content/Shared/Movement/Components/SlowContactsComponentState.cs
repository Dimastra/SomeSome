using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002F4 RID: 756
	[NetSerializable]
	[Serializable]
	public sealed class SlowContactsComponentState : ComponentState
	{
		// Token: 0x06000880 RID: 2176 RVA: 0x0001CCD4 File Offset: 0x0001AED4
		public SlowContactsComponentState(float walkSpeedModifier, float sprintSpeedModifier)
		{
			this.WalkSpeedModifier = walkSpeedModifier;
			this.SprintSpeedModifier = sprintSpeedModifier;
		}

		// Token: 0x040008A0 RID: 2208
		public readonly float WalkSpeedModifier;

		// Token: 0x040008A1 RID: 2209
		public readonly float SprintSpeedModifier;
	}
}
