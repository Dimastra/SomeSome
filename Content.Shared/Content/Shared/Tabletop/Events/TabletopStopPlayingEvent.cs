using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop.Events
{
	// Token: 0x020000F2 RID: 242
	[NetSerializable]
	[Serializable]
	public sealed class TabletopStopPlayingEvent : EntityEventArgs
	{
		// Token: 0x060002B9 RID: 697 RVA: 0x0000CB80 File Offset: 0x0000AD80
		public TabletopStopPlayingEvent(EntityUid tableUid)
		{
			this.TableUid = tableUid;
		}

		// Token: 0x04000307 RID: 775
		public EntityUid TableUid;
	}
}
