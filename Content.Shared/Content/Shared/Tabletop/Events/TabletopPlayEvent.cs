using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop.Events
{
	// Token: 0x020000F1 RID: 241
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class TabletopPlayEvent : EntityEventArgs
	{
		// Token: 0x060002B8 RID: 696 RVA: 0x0000CB5B File Offset: 0x0000AD5B
		public TabletopPlayEvent(EntityUid tableUid, EntityUid cameraUid, string title, Vector2i size)
		{
			this.TableUid = tableUid;
			this.CameraUid = cameraUid;
			this.Title = title;
			this.Size = size;
		}

		// Token: 0x04000303 RID: 771
		public EntityUid TableUid;

		// Token: 0x04000304 RID: 772
		public EntityUid CameraUid;

		// Token: 0x04000305 RID: 773
		public string Title;

		// Token: 0x04000306 RID: 774
		public Vector2i Size;
	}
}
