using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Events
{
	// Token: 0x02000056 RID: 86
	[NetSerializable]
	[Serializable]
	public sealed class RequestShootEvent : EntityEventArgs
	{
		// Token: 0x04000104 RID: 260
		public EntityUid Gun;

		// Token: 0x04000105 RID: 261
		public EntityCoordinates Coordinates;
	}
}
