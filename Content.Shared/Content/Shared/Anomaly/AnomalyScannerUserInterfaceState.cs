using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Anomaly
{
	// Token: 0x020006FF RID: 1791
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AnomalyScannerUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06001580 RID: 5504 RVA: 0x0004635A File Offset: 0x0004455A
		public AnomalyScannerUserInterfaceState(FormattedMessage message, TimeSpan? nextPulseTime)
		{
			this.Message = message;
			this.NextPulseTime = nextPulseTime;
		}

		// Token: 0x040015C9 RID: 5577
		public FormattedMessage Message;

		// Token: 0x040015CA RID: 5578
		public TimeSpan? NextPulseTime;
	}
}
