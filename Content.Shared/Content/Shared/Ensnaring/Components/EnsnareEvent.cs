using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Ensnaring.Components
{
	// Token: 0x020004BF RID: 1215
	public sealed class EnsnareEvent : EntityEventArgs
	{
		// Token: 0x06000EB0 RID: 3760 RVA: 0x0002F4B7 File Offset: 0x0002D6B7
		public EnsnareEvent(float walkSpeed, float sprintSpeed)
		{
			this.WalkSpeed = walkSpeed;
			this.SprintSpeed = sprintSpeed;
		}

		// Token: 0x04000DD7 RID: 3543
		public readonly float WalkSpeed;

		// Token: 0x04000DD8 RID: 3544
		public readonly float SprintSpeed;
	}
}
