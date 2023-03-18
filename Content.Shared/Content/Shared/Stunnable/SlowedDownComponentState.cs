using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Stunnable
{
	// Token: 0x02000110 RID: 272
	[NetSerializable]
	[Serializable]
	public sealed class SlowedDownComponentState : ComponentState
	{
		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000324 RID: 804 RVA: 0x0000DEEB File Offset: 0x0000C0EB
		// (set) Token: 0x06000325 RID: 805 RVA: 0x0000DEF3 File Offset: 0x0000C0F3
		public float SprintSpeedModifier { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000326 RID: 806 RVA: 0x0000DEFC File Offset: 0x0000C0FC
		// (set) Token: 0x06000327 RID: 807 RVA: 0x0000DF04 File Offset: 0x0000C104
		public float WalkSpeedModifier { get; set; }

		// Token: 0x06000328 RID: 808 RVA: 0x0000DF0D File Offset: 0x0000C10D
		public SlowedDownComponentState(float sprintSpeedModifier, float walkSpeedModifier)
		{
			this.SprintSpeedModifier = sprintSpeedModifier;
			this.WalkSpeedModifier = walkSpeedModifier;
		}
	}
}
