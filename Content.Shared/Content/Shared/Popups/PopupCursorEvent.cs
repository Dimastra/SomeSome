using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Popups
{
	// Token: 0x0200025F RID: 607
	[NetSerializable]
	[Serializable]
	public sealed class PopupCursorEvent : PopupEvent
	{
		// Token: 0x06000705 RID: 1797 RVA: 0x0001846C File Offset: 0x0001666C
		[NullableContext(1)]
		public PopupCursorEvent(string message, PopupType type) : base(message, type)
		{
		}
	}
}
