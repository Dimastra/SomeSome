using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking
{
	// Token: 0x02000468 RID: 1128
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class TickerLobbyInfoEvent : EntityEventArgs
	{
		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06000DAE RID: 3502 RVA: 0x0002CA94 File Offset: 0x0002AC94
		public string TextBlob { get; }

		// Token: 0x06000DAF RID: 3503 RVA: 0x0002CA9C File Offset: 0x0002AC9C
		public TickerLobbyInfoEvent(string textBlob)
		{
			this.TextBlob = textBlob;
		}
	}
}
