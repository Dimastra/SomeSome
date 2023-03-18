using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Chemistry.UI
{
	// Token: 0x020003DD RID: 989
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DispenseReagentButton : Button
	{
		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x0600186E RID: 6254 RVA: 0x0008D323 File Offset: 0x0008B523
		public string ReagentId { get; }

		// Token: 0x0600186F RID: 6255 RVA: 0x0008D32B File Offset: 0x0008B52B
		public DispenseReagentButton(string reagentId, string text)
		{
			this.ReagentId = reagentId;
			base.Text = text;
		}
	}
}
