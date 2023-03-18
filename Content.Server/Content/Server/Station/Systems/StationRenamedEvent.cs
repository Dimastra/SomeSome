using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Station.Systems
{
	// Token: 0x0200019F RID: 415
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StationRenamedEvent : EntityEventArgs
	{
		// Token: 0x06000852 RID: 2130 RVA: 0x0002A9A3 File Offset: 0x00028BA3
		public StationRenamedEvent(string oldName, string newName)
		{
			this.OldName = oldName;
			this.NewName = newName;
		}

		// Token: 0x04000513 RID: 1299
		public string OldName;

		// Token: 0x04000514 RID: 1300
		public string NewName;
	}
}
