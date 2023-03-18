using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Doors.Components
{
	// Token: 0x020004EB RID: 1259
	[NetSerializable]
	[Serializable]
	public sealed class AirlockComponentState : ComponentState
	{
		// Token: 0x06000F51 RID: 3921 RVA: 0x000315F4 File Offset: 0x0002F7F4
		public AirlockComponentState(bool safety)
		{
			this.Safety = safety;
		}

		// Token: 0x04000E54 RID: 3668
		public readonly bool Safety;
	}
}
