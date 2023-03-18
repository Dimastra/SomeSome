using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges
{
	// Token: 0x02000624 RID: 1572
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class NotekeeperUiMessageEvent : CartridgeMessageEvent
	{
		// Token: 0x0600130D RID: 4877 RVA: 0x0003F9ED File Offset: 0x0003DBED
		public NotekeeperUiMessageEvent(NotekeeperUiAction action, string note)
		{
			this.Action = action;
			this.Note = note;
		}

		// Token: 0x040012ED RID: 4845
		public readonly NotekeeperUiAction Action;

		// Token: 0x040012EE RID: 4846
		public readonly string Note;
	}
}
