using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.CombatMode.Disarm
{
	// Token: 0x02000634 RID: 1588
	[RegisterComponent]
	public sealed class DisarmMalusComponent : Component
	{
		// Token: 0x040014BA RID: 5306
		[DataField("malus", false, 1, false, false, null)]
		public float Malus = 0.3f;
	}
}
