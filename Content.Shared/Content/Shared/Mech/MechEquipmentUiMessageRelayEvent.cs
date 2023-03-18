using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mech
{
	// Token: 0x02000315 RID: 789
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MechEquipmentUiMessageRelayEvent : EntityEventArgs
	{
		// Token: 0x06000912 RID: 2322 RVA: 0x0001E7DB File Offset: 0x0001C9DB
		public MechEquipmentUiMessageRelayEvent(MechEquipmentUiMessage message)
		{
			this.Message = message;
		}

		// Token: 0x04000907 RID: 2311
		public MechEquipmentUiMessage Message;
	}
}
