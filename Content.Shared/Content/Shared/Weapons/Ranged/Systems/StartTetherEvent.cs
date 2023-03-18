using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Systems
{
	// Token: 0x0200004F RID: 79
	[NetSerializable]
	[Serializable]
	public sealed class StartTetherEvent : EntityEventArgs
	{
		// Token: 0x040000FA RID: 250
		public EntityUid Entity;

		// Token: 0x040000FB RID: 251
		public MapCoordinates Coordinates;
	}
}
