using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Popups
{
	// Token: 0x0200025E RID: 606
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public abstract class PopupEvent : EntityEventArgs
	{
		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000702 RID: 1794 RVA: 0x00018446 File Offset: 0x00016646
		public string Message { get; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000703 RID: 1795 RVA: 0x0001844E File Offset: 0x0001664E
		public PopupType Type { get; }

		// Token: 0x06000704 RID: 1796 RVA: 0x00018456 File Offset: 0x00016656
		protected PopupEvent(string message, PopupType type)
		{
			this.Message = message;
			this.Type = type;
		}
	}
}
