using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Events
{
	// Token: 0x02000753 RID: 1875
	[NetSerializable]
	[Serializable]
	public sealed class FullPlayerListEvent : EntityEventArgs
	{
		// Token: 0x040016F2 RID: 5874
		[Nullable(1)]
		public List<PlayerInfo> PlayersInfo = new List<PlayerInfo>();
	}
}
