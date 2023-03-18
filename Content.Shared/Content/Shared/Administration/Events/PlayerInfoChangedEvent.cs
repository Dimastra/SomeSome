using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Events
{
	// Token: 0x02000754 RID: 1876
	[NetSerializable]
	[Serializable]
	public sealed class PlayerInfoChangedEvent : EntityEventArgs
	{
		// Token: 0x040016F3 RID: 5875
		[Nullable(2)]
		public PlayerInfo PlayerInfo;
	}
}
