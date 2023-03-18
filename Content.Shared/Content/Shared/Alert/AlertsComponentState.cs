using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Alert
{
	// Token: 0x0200071F RID: 1823
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AlertsComponentState : ComponentState
	{
		// Token: 0x06001618 RID: 5656 RVA: 0x000484CC File Offset: 0x000466CC
		public AlertsComponentState(Dictionary<AlertKey, AlertState> alerts)
		{
			this.Alerts = alerts;
		}

		// Token: 0x04001651 RID: 5713
		public Dictionary<AlertKey, AlertState> Alerts;
	}
}
