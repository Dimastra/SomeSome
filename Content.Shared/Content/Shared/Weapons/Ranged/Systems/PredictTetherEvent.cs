using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Systems
{
	// Token: 0x02000052 RID: 82
	[NetSerializable]
	[Serializable]
	public sealed class PredictTetherEvent : EntityEventArgs
	{
		// Token: 0x040000FD RID: 253
		public EntityUid Entity;
	}
}
