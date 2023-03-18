using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Events
{
	// Token: 0x0200062F RID: 1583
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CargoConsoleAddOrderMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001320 RID: 4896 RVA: 0x0003FC50 File Offset: 0x0003DE50
		public CargoConsoleAddOrderMessage(string requester, string reason, string productId, int amount)
		{
			this.Requester = requester;
			this.Reason = reason;
			this.ProductId = productId;
			this.Amount = amount;
		}

		// Token: 0x0400130E RID: 4878
		public string Requester;

		// Token: 0x0400130F RID: 4879
		public string Reason;

		// Token: 0x04001310 RID: 4880
		public string ProductId;

		// Token: 0x04001311 RID: 4881
		public int Amount;
	}
}
