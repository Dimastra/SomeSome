using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Doors
{
	// Token: 0x020004E7 RID: 1255
	public sealed class BeforeDoorPryEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000F27 RID: 3879 RVA: 0x0003087B File Offset: 0x0002EA7B
		public BeforeDoorPryEvent(EntityUid user, EntityUid tool)
		{
			this.User = user;
			this.Tool = tool;
		}

		// Token: 0x04000E35 RID: 3637
		public readonly EntityUid User;

		// Token: 0x04000E36 RID: 3638
		public readonly EntityUid Tool;
	}
}
