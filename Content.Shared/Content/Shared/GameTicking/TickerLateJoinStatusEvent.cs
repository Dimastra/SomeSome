using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking
{
	// Token: 0x02000466 RID: 1126
	[NetSerializable]
	[Serializable]
	public sealed class TickerLateJoinStatusEvent : EntityEventArgs
	{
		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06000DA4 RID: 3492 RVA: 0x0002CA08 File Offset: 0x0002AC08
		public bool Disallowed { get; }

		// Token: 0x06000DA5 RID: 3493 RVA: 0x0002CA10 File Offset: 0x0002AC10
		public TickerLateJoinStatusEvent(bool disallowed)
		{
			this.Disallowed = disallowed;
		}
	}
}
