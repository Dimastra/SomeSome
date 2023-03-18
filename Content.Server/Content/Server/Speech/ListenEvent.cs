using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Speech
{
	// Token: 0x020001AF RID: 431
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ListenEvent : EntityEventArgs
	{
		// Token: 0x0600087D RID: 2173 RVA: 0x0002B4EA File Offset: 0x000296EA
		public ListenEvent(string message, EntityUid source)
		{
			this.Message = message;
			this.Source = source;
		}

		// Token: 0x0400052F RID: 1327
		public readonly string Message;

		// Token: 0x04000530 RID: 1328
		public readonly EntityUid Source;
	}
}
