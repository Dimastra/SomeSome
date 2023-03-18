using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Ensnaring.Components
{
	// Token: 0x020004BE RID: 1214
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class EnsnaringComponent : Component
	{
		// Token: 0x04000DD0 RID: 3536
		[ViewVariables]
		[DataField("freeTime", false, 1, false, false, null)]
		public float FreeTime = 3.5f;

		// Token: 0x04000DD1 RID: 3537
		[ViewVariables]
		[DataField("breakoutTime", false, 1, false, false, null)]
		public float BreakoutTime = 30f;

		// Token: 0x04000DD2 RID: 3538
		[ViewVariables]
		[DataField("walkSpeed", false, 1, false, false, null)]
		public float WalkSpeed = 0.9f;

		// Token: 0x04000DD3 RID: 3539
		[ViewVariables]
		[DataField("sprintSpeed", false, 1, false, false, null)]
		public float SprintSpeed = 0.9f;

		// Token: 0x04000DD4 RID: 3540
		[ViewVariables]
		[DataField("canThrowTrigger", false, 1, false, false, null)]
		public bool CanThrowTrigger;

		// Token: 0x04000DD5 RID: 3541
		[ViewVariables]
		[DataField("ensnared", false, 1, false, false, null)]
		public EntityUid? Ensnared;

		// Token: 0x04000DD6 RID: 3542
		[ViewVariables]
		[DataField("canMoveBreakout", false, 1, false, false, null)]
		public bool CanMoveBreakout;
	}
}
