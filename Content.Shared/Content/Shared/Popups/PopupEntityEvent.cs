using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Popups
{
	// Token: 0x02000261 RID: 609
	[NetSerializable]
	[Serializable]
	public sealed class PopupEntityEvent : PopupEvent
	{
		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000708 RID: 1800 RVA: 0x0001848F File Offset: 0x0001668F
		public EntityUid Uid { get; }

		// Token: 0x06000709 RID: 1801 RVA: 0x00018497 File Offset: 0x00016697
		[NullableContext(1)]
		public PopupEntityEvent(string message, PopupType type, EntityUid uid) : base(message, type)
		{
			this.Uid = uid;
		}
	}
}
