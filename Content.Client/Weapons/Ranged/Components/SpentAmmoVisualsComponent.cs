using System;
using System.Runtime.CompilerServices;
using Content.Client.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Weapons.Ranged.Components
{
	// Token: 0x02000039 RID: 57
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(GunSystem)
	})]
	public sealed class SpentAmmoVisualsComponent : Component
	{
		// Token: 0x040000A6 RID: 166
		[DataField("suffix", false, 1, false, false, null)]
		public bool Suffix = true;

		// Token: 0x040000A7 RID: 167
		[Nullable(1)]
		[DataField("state", false, 1, false, false, null)]
		public string State = "base";
	}
}
