using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.BUI
{
	// Token: 0x02000636 RID: 1590
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CargoShuttleConsoleBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06001327 RID: 4903 RVA: 0x0003FCEE File Offset: 0x0003DEEE
		public CargoShuttleConsoleBoundUserInterfaceState(string accountName, string shuttleName, bool canRecall, TimeSpan? shuttleETA, List<CargoOrderData> orders)
		{
			this.AccountName = accountName;
			this.ShuttleName = shuttleName;
			this.CanRecall = canRecall;
			this.ShuttleETA = shuttleETA;
			this.Orders = orders;
		}

		// Token: 0x0400131F RID: 4895
		public string AccountName;

		// Token: 0x04001320 RID: 4896
		public string ShuttleName;

		// Token: 0x04001321 RID: 4897
		public bool CanRecall;

		// Token: 0x04001322 RID: 4898
		public TimeSpan? ShuttleETA;

		// Token: 0x04001323 RID: 4899
		public List<CargoOrderData> Orders;
	}
}
