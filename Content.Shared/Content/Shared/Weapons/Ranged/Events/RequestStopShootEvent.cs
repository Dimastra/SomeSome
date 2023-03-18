using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Events
{
	// Token: 0x02000057 RID: 87
	[NetSerializable]
	[Serializable]
	public sealed class RequestStopShootEvent : EntityEventArgs
	{
		// Token: 0x04000106 RID: 262
		public EntityUid Gun;
	}
}
