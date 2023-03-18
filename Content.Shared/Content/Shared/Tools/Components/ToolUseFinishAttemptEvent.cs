using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Tools.Components
{
	// Token: 0x020000BF RID: 191
	public sealed class ToolUseFinishAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x0600021A RID: 538 RVA: 0x0000AA83 File Offset: 0x00008C83
		public float Fuel { get; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600021B RID: 539 RVA: 0x0000AA8B File Offset: 0x00008C8B
		public EntityUid User { get; }

		// Token: 0x0600021C RID: 540 RVA: 0x0000AA93 File Offset: 0x00008C93
		public ToolUseFinishAttemptEvent(float fuel, EntityUid user)
		{
			this.Fuel = fuel;
		}
	}
}
