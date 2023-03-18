using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x0200073F RID: 1855
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class QuickDialogOpenEvent : EntityEventArgs
	{
		// Token: 0x0600168F RID: 5775 RVA: 0x00049A83 File Offset: 0x00047C83
		public QuickDialogOpenEvent(string title, List<QuickDialogEntry> prompts, int dialogId, QuickDialogButtonFlag buttons)
		{
			this.Title = title;
			this.Prompts = prompts;
			this.Buttons = buttons;
			this.DialogId = dialogId;
		}

		// Token: 0x040016BF RID: 5823
		public string Title;

		// Token: 0x040016C0 RID: 5824
		public int DialogId;

		// Token: 0x040016C1 RID: 5825
		public List<QuickDialogEntry> Prompts;

		// Token: 0x040016C2 RID: 5826
		public QuickDialogButtonFlag Buttons = QuickDialogButtonFlag.OkButton;
	}
}
