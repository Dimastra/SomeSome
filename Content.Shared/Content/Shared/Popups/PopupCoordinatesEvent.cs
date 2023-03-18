using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Popups
{
	// Token: 0x02000260 RID: 608
	[NetSerializable]
	[Serializable]
	public sealed class PopupCoordinatesEvent : PopupEvent
	{
		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000706 RID: 1798 RVA: 0x00018476 File Offset: 0x00016676
		public EntityCoordinates Coordinates { get; }

		// Token: 0x06000707 RID: 1799 RVA: 0x0001847E File Offset: 0x0001667E
		[NullableContext(1)]
		public PopupCoordinatesEvent(string message, PopupType type, EntityCoordinates coordinates) : base(message, type)
		{
			this.Coordinates = coordinates;
		}
	}
}
