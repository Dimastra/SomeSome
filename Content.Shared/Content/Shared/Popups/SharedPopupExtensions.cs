using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Popups
{
	// Token: 0x0200025C RID: 604
	[NullableContext(1)]
	[Nullable(0)]
	public static class SharedPopupExtensions
	{
		// Token: 0x060006F3 RID: 1779 RVA: 0x00018415 File Offset: 0x00016615
		[Obsolete("Use PopupSystem.PopupEntity instead.")]
		public static void PopupMessage(this EntityUid source, EntityUid viewer, string message)
		{
			EntitySystem.Get<SharedPopupSystem>().PopupEntity(message, source, viewer, PopupType.Small);
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x00018425 File Offset: 0x00016625
		[Obsolete("Use PopupSystem.PopupEntity instead.")]
		public static void PopupMessage(this EntityUid viewer, string message)
		{
			viewer.PopupMessage(viewer, message);
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x0001842F File Offset: 0x0001662F
		[Obsolete("Use PopupSystem.PopupCursor instead.")]
		public static void PopupMessageCursor(this EntityUid viewer, string message)
		{
			EntitySystem.Get<SharedPopupSystem>().PopupCursor(message, viewer, PopupType.Small);
		}
	}
}
