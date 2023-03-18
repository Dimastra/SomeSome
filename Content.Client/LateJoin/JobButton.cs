using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.LateJoin
{
	// Token: 0x0200028A RID: 650
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class JobButton : ContainerButton
	{
		// Token: 0x17000390 RID: 912
		// (get) Token: 0x0600108F RID: 4239 RVA: 0x000634BC File Offset: 0x000616BC
		public string JobId { get; }

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06001090 RID: 4240 RVA: 0x000634C4 File Offset: 0x000616C4
		public uint? Amount { get; }

		// Token: 0x06001091 RID: 4241 RVA: 0x000634CC File Offset: 0x000616CC
		public JobButton(string jobId, uint? amount)
		{
			this.JobId = jobId;
			this.Amount = amount;
			base.AddStyleClass("button");
		}
	}
}
