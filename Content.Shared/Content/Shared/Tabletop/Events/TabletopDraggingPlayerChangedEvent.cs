using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop.Events
{
	// Token: 0x020000EF RID: 239
	[NetSerializable]
	[Serializable]
	public sealed class TabletopDraggingPlayerChangedEvent : EntityEventArgs
	{
		// Token: 0x060002B3 RID: 691 RVA: 0x0000CB10 File Offset: 0x0000AD10
		public TabletopDraggingPlayerChangedEvent(EntityUid draggedEntityUid, bool isDragging)
		{
			this.DraggedEntityUid = draggedEntityUid;
			this.IsDragging = isDragging;
		}

		// Token: 0x040002FE RID: 766
		public EntityUid DraggedEntityUid;

		// Token: 0x040002FF RID: 767
		public bool IsDragging;
	}
}
