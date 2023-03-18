using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DragDrop
{
	// Token: 0x020004D9 RID: 1241
	[NetSerializable]
	[Serializable]
	public sealed class DragDropRequestEvent : EntityEventArgs
	{
		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06000EF2 RID: 3826 RVA: 0x0002FF6F File Offset: 0x0002E16F
		public EntityUid Dragged { get; }

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06000EF3 RID: 3827 RVA: 0x0002FF77 File Offset: 0x0002E177
		public EntityUid Target { get; }

		// Token: 0x06000EF4 RID: 3828 RVA: 0x0002FF7F File Offset: 0x0002E17F
		public DragDropRequestEvent(EntityUid dragged, EntityUid target)
		{
			this.Dragged = dragged;
			this.Target = target;
		}
	}
}
