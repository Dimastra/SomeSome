using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Communications
{
	// Token: 0x02000593 RID: 1427
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CommunicationsConsoleInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06001177 RID: 4471 RVA: 0x00039224 File Offset: 0x00037424
		public CommunicationsConsoleInterfaceState(bool canAnnounce, bool canCall, [Nullable(new byte[]
		{
			2,
			1
		})] List<string> alertLevels, string currentAlert, float currentAlertDelay, TimeSpan? expectedCountdownEnd = null)
		{
			this.CanAnnounce = canAnnounce;
			this.CanCall = canCall;
			this.ExpectedCountdownEnd = expectedCountdownEnd;
			this.CountdownStarted = (expectedCountdownEnd != null);
			this.AlertLevels = alertLevels;
			this.CurrentAlert = currentAlert;
			this.CurrentAlertDelay = currentAlertDelay;
		}

		// Token: 0x04001020 RID: 4128
		public readonly bool CanAnnounce;

		// Token: 0x04001021 RID: 4129
		public readonly bool CanCall;

		// Token: 0x04001022 RID: 4130
		public readonly TimeSpan? ExpectedCountdownEnd;

		// Token: 0x04001023 RID: 4131
		public readonly bool CountdownStarted;

		// Token: 0x04001024 RID: 4132
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public List<string> AlertLevels;

		// Token: 0x04001025 RID: 4133
		public string CurrentAlert;

		// Token: 0x04001026 RID: 4134
		public float CurrentAlertDelay;
	}
}
