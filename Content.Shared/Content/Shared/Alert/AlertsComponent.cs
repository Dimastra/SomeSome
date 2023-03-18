using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Alert
{
	// Token: 0x0200071E RID: 1822
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class AlertsComponent : Component
	{
		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06001616 RID: 5654 RVA: 0x000484B6 File Offset: 0x000466B6
		public override bool SendOnlyToOwner
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04001650 RID: 5712
		[Nullable(1)]
		[ViewVariables]
		public Dictionary<AlertKey, AlertState> Alerts = new Dictionary<AlertKey, AlertState>();
	}
}
