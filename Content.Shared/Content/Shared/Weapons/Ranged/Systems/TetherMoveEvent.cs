using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Systems
{
	// Token: 0x02000051 RID: 81
	[NetSerializable]
	[Serializable]
	public sealed class TetherMoveEvent : EntityEventArgs
	{
		// Token: 0x040000FC RID: 252
		public MapCoordinates Coordinates;
	}
}
