using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Chemistry.UI
{
	// Token: 0x020003D7 RID: 983
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReagentButton : Button
	{
		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06001833 RID: 6195 RVA: 0x0008BDF3 File Offset: 0x00089FF3
		// (set) Token: 0x06001834 RID: 6196 RVA: 0x0008BDFB File Offset: 0x00089FFB
		public ChemMasterReagentAmount Amount { get; set; }

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06001835 RID: 6197 RVA: 0x0008BE04 File Offset: 0x0008A004
		// (set) Token: 0x06001836 RID: 6198 RVA: 0x0008BE0C File Offset: 0x0008A00C
		public string Id { get; set; }

		// Token: 0x06001837 RID: 6199 RVA: 0x0008BE15 File Offset: 0x0008A015
		public ReagentButton(string text, ChemMasterReagentAmount amount, string id, bool isBuffer, string styleClass)
		{
			base.AddStyleClass(styleClass);
			base.Text = text;
			this.Amount = amount;
			this.Id = id;
			this.IsBuffer = isBuffer;
		}

		// Token: 0x04000C60 RID: 3168
		public bool IsBuffer = true;
	}
}
