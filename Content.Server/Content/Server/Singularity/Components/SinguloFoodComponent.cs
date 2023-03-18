using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Singularity.Components
{
	// Token: 0x020001F3 RID: 499
	[RegisterComponent]
	public sealed class SinguloFoodComponent : Component
	{
		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060009B0 RID: 2480 RVA: 0x00031136 File Offset: 0x0002F336
		// (set) Token: 0x060009B1 RID: 2481 RVA: 0x0003113E File Offset: 0x0002F33E
		[ViewVariables]
		[DataField("energy", false, 1, false, false, null)]
		public float Energy { get; set; } = 1f;
	}
}
