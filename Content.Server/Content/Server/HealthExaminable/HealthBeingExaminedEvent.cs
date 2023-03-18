using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Utility;

namespace Content.Server.HealthExaminable
{
	// Token: 0x02000477 RID: 1143
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HealthBeingExaminedEvent
	{
		// Token: 0x060016D7 RID: 5847 RVA: 0x00078518 File Offset: 0x00076718
		public HealthBeingExaminedEvent(FormattedMessage message)
		{
			this.Message = message;
		}

		// Token: 0x04000E56 RID: 3670
		public FormattedMessage Message;
	}
}
