using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking
{
	// Token: 0x0200034D RID: 845
	[NetSerializable]
	[Serializable]
	public sealed class SignalTimerDelayChangedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x170001EA RID: 490
		// (get) Token: 0x060009ED RID: 2541 RVA: 0x00020718 File Offset: 0x0001E918
		public TimeSpan Delay { get; }

		// Token: 0x060009EE RID: 2542 RVA: 0x00020720 File Offset: 0x0001E920
		public SignalTimerDelayChangedMessage(TimeSpan delay)
		{
			this.Delay = delay;
		}
	}
}
