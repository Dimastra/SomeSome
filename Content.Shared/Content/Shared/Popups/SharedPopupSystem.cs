using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Players;

namespace Content.Shared.Popups
{
	// Token: 0x0200025D RID: 605
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedPopupSystem : EntitySystem
	{
		// Token: 0x060006F6 RID: 1782
		public abstract void PopupCursor(string message, PopupType type = PopupType.Small);

		// Token: 0x060006F7 RID: 1783
		public abstract void PopupCursor(string message, ICommonSession recipient, PopupType type = PopupType.Small);

		// Token: 0x060006F8 RID: 1784
		public abstract void PopupCursor(string message, EntityUid recipient, PopupType type = PopupType.Small);

		// Token: 0x060006F9 RID: 1785
		public abstract void PopupCoordinates(string message, EntityCoordinates coordinates, PopupType type = PopupType.Small);

		// Token: 0x060006FA RID: 1786
		public abstract void PopupCoordinates(string message, EntityCoordinates coordinates, Filter filter, bool recordReplay, PopupType type = PopupType.Small);

		// Token: 0x060006FB RID: 1787
		public abstract void PopupCoordinates(string message, EntityCoordinates coordinates, EntityUid recipient, PopupType type = PopupType.Small);

		// Token: 0x060006FC RID: 1788
		public abstract void PopupCoordinates(string message, EntityCoordinates coordinates, ICommonSession recipient, PopupType type = PopupType.Small);

		// Token: 0x060006FD RID: 1789
		public abstract void PopupEntity(string message, EntityUid uid, PopupType type = PopupType.Small);

		// Token: 0x060006FE RID: 1790
		public abstract void PopupEntity(string message, EntityUid uid, EntityUid recipient, PopupType type = PopupType.Small);

		// Token: 0x060006FF RID: 1791
		public abstract void PopupEntity(string message, EntityUid uid, ICommonSession recipient, PopupType type = PopupType.Small);

		// Token: 0x06000700 RID: 1792
		public abstract void PopupEntity(string message, EntityUid uid, Filter filter, bool recordReplay, PopupType type = PopupType.Small);
	}
}
