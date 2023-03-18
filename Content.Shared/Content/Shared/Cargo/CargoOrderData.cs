using System;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Access.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo
{
	// Token: 0x02000627 RID: 1575
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CargoOrderData
	{
		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x0600130F RID: 4879 RVA: 0x0003FA12 File Offset: 0x0003DC12
		public int PrintableOrderNumber
		{
			get
			{
				return this.OrderIndex + 1;
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06001310 RID: 4880 RVA: 0x0003FA1C File Offset: 0x0003DC1C
		public bool Approved
		{
			get
			{
				return this.Approver != null;
			}
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x0003FA2A File Offset: 0x0003DC2A
		public CargoOrderData(int orderIndex, string productId, int amount, string requester, string reason)
		{
			this.OrderIndex = orderIndex;
			this.ProductId = productId;
			this.Amount = amount;
			this.Requester = requester;
			this.Reason = reason;
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x0003FA58 File Offset: 0x0003DC58
		[NullableContext(2)]
		public void SetApproverData(IdCardComponent idCard)
		{
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrWhiteSpace((idCard != null) ? idCard.FullName : null))
			{
				StringBuilder stringBuilder = sb;
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder);
				appendInterpolatedStringHandler.AppendFormatted(idCard.FullName);
				appendInterpolatedStringHandler.AppendLiteral(" ");
				stringBuilder2.Append(ref appendInterpolatedStringHandler);
			}
			if (!string.IsNullOrWhiteSpace((idCard != null) ? idCard.JobTitle : null))
			{
				StringBuilder stringBuilder = sb;
				StringBuilder stringBuilder3 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder);
				appendInterpolatedStringHandler.AppendLiteral("(");
				appendInterpolatedStringHandler.AppendFormatted(idCard.JobTitle);
				appendInterpolatedStringHandler.AppendLiteral(")");
				stringBuilder3.Append(ref appendInterpolatedStringHandler);
			}
			this.Approver = sb.ToString();
		}

		// Token: 0x040012F3 RID: 4851
		public int OrderIndex;

		// Token: 0x040012F4 RID: 4852
		public string ProductId;

		// Token: 0x040012F5 RID: 4853
		public int Amount;

		// Token: 0x040012F6 RID: 4854
		public string Requester;

		// Token: 0x040012F7 RID: 4855
		public string Reason;

		// Token: 0x040012F8 RID: 4856
		[Nullable(2)]
		public string Approver;
	}
}
