using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Store
{
	// Token: 0x02000129 RID: 297
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class StoreRequestWithdrawMessage : BoundUserInterfaceMessage
	{
		// Token: 0x0600036A RID: 874 RVA: 0x0000E776 File Offset: 0x0000C976
		public StoreRequestWithdrawMessage(string currency, int amount)
		{
			this.Currency = currency;
			this.Amount = amount;
		}

		// Token: 0x04000389 RID: 905
		public string Currency;

		// Token: 0x0400038A RID: 906
		public int Amount;
	}
}
