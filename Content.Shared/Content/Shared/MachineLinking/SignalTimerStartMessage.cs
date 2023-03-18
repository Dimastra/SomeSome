using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking
{
	// Token: 0x0200034E RID: 846
	[NetSerializable]
	[Serializable]
	public sealed class SignalTimerStartMessage : BoundUserInterfaceMessage
	{
		// Token: 0x170001EB RID: 491
		// (get) Token: 0x060009EF RID: 2543 RVA: 0x0002072F File Offset: 0x0001E92F
		public EntityUid User { get; }

		// Token: 0x060009F0 RID: 2544 RVA: 0x00020737 File Offset: 0x0001E937
		public SignalTimerStartMessage(EntityUid user)
		{
			this.User = user;
		}
	}
}
