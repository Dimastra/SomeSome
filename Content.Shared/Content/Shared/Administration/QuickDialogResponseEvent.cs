using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x02000740 RID: 1856
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class QuickDialogResponseEvent : EntityEventArgs
	{
		// Token: 0x06001690 RID: 5776 RVA: 0x00049AAF File Offset: 0x00047CAF
		public QuickDialogResponseEvent(int dialogId, Dictionary<string, string> responses, QuickDialogButtonFlag buttonPressed)
		{
			this.DialogId = dialogId;
			this.Responses = responses;
			this.ButtonPressed = buttonPressed;
		}

		// Token: 0x040016C3 RID: 5827
		public int DialogId;

		// Token: 0x040016C4 RID: 5828
		public Dictionary<string, string> Responses;

		// Token: 0x040016C5 RID: 5829
		public QuickDialogButtonFlag ButtonPressed;
	}
}
