using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking
{
	// Token: 0x0200034C RID: 844
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SignalTimerTextChangedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x060009EB RID: 2539 RVA: 0x00020701 File Offset: 0x0001E901
		public string Text { get; }

		// Token: 0x060009EC RID: 2540 RVA: 0x00020709 File Offset: 0x0001E909
		public SignalTimerTextChangedMessage(string text)
		{
			this.Text = text;
		}
	}
}
