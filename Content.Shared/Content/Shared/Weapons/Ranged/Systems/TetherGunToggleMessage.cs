using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Systems
{
	// Token: 0x0200004E RID: 78
	[NetSerializable]
	[Serializable]
	public sealed class TetherGunToggleMessage : EntityEventArgs
	{
		// Token: 0x040000F9 RID: 249
		public bool Enabled;
	}
}
