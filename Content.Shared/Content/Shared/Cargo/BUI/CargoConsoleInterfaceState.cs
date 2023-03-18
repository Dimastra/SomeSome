using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.BUI
{
	// Token: 0x02000635 RID: 1589
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CargoConsoleInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06001326 RID: 4902 RVA: 0x0003FCC1 File Offset: 0x0003DEC1
		public CargoConsoleInterfaceState(string name, int count, int capacity, int balance, List<CargoOrderData> orders)
		{
			this.Name = name;
			this.Count = count;
			this.Capacity = capacity;
			this.Balance = balance;
			this.Orders = orders;
		}

		// Token: 0x0400131A RID: 4890
		public string Name;

		// Token: 0x0400131B RID: 4891
		public int Count;

		// Token: 0x0400131C RID: 4892
		public int Capacity;

		// Token: 0x0400131D RID: 4893
		public int Balance;

		// Token: 0x0400131E RID: 4894
		public List<CargoOrderData> Orders;
	}
}
