using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Weapons.Ranged.Components
{
	// Token: 0x020000B4 RID: 180
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ChemicalAmmoComponent : Component
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x0000FE46 File Offset: 0x0000E046
		// (set) Token: 0x060002E6 RID: 742 RVA: 0x0000FE4E File Offset: 0x0000E04E
		[DataField("solution", false, 1, false, false, null)]
		public string SolutionName { get; set; } = "ammo";

		// Token: 0x040001F5 RID: 501
		public const string DefaultSolutionName = "ammo";
	}
}
