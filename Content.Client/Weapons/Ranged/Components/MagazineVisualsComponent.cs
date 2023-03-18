using System;
using System.Runtime.CompilerServices;
using Content.Client.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Weapons.Ranged.Components
{
	// Token: 0x02000037 RID: 55
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(GunSystem)
	})]
	public sealed class MagazineVisualsComponent : Component
	{
		// Token: 0x0400009C RID: 156
		[Nullable(2)]
		[DataField("magState", false, 1, false, false, null)]
		public string MagState;

		// Token: 0x0400009D RID: 157
		[DataField("steps", false, 1, false, false, null)]
		public int MagSteps;

		// Token: 0x0400009E RID: 158
		[DataField("zeroVisible", false, 1, false, false, null)]
		public bool ZeroVisible;
	}
}
