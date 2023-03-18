using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Tools.Components
{
	// Token: 0x020000BD RID: 189
	public sealed class ToolUseAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000215 RID: 533 RVA: 0x0000AA3D File Offset: 0x00008C3D
		// (set) Token: 0x06000216 RID: 534 RVA: 0x0000AA45 File Offset: 0x00008C45
		public float Fuel { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000217 RID: 535 RVA: 0x0000AA4E File Offset: 0x00008C4E
		public EntityUid User { get; }

		// Token: 0x06000218 RID: 536 RVA: 0x0000AA56 File Offset: 0x00008C56
		public ToolUseAttemptEvent(float fuel, EntityUid user)
		{
			this.Fuel = fuel;
			this.User = user;
		}
	}
}
