using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Alert
{
	// Token: 0x02000721 RID: 1825
	[NetSerializable]
	[Serializable]
	public struct AlertState
	{
		// Token: 0x04001654 RID: 5716
		public short? Severity;

		// Token: 0x04001655 RID: 5717
		public ValueTuple<TimeSpan, TimeSpan>? Cooldown;

		// Token: 0x04001656 RID: 5718
		public AlertType Type;
	}
}
