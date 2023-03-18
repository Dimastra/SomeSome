using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop.Events
{
	// Token: 0x020000F0 RID: 240
	[NetSerializable]
	[Serializable]
	public sealed class TabletopMoveEvent : EntityEventArgs
	{
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x0000CB26 File Offset: 0x0000AD26
		public EntityUid MovedEntityUid { get; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060002B5 RID: 693 RVA: 0x0000CB2E File Offset: 0x0000AD2E
		public MapCoordinates Coordinates { get; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060002B6 RID: 694 RVA: 0x0000CB36 File Offset: 0x0000AD36
		public EntityUid TableUid { get; }

		// Token: 0x060002B7 RID: 695 RVA: 0x0000CB3E File Offset: 0x0000AD3E
		public TabletopMoveEvent(EntityUid movedEntityUid, MapCoordinates coordinates, EntityUid tableUid)
		{
			this.MovedEntityUid = movedEntityUid;
			this.Coordinates = coordinates;
			this.TableUid = tableUid;
		}
	}
}
