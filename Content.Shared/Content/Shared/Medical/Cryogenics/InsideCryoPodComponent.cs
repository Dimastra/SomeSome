using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Medical.Cryogenics
{
	// Token: 0x0200030E RID: 782
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class InsideCryoPodComponent : Component
	{
		// Token: 0x170001AE RID: 430
		// (get) Token: 0x060008F3 RID: 2291 RVA: 0x0001E355 File Offset: 0x0001C555
		// (set) Token: 0x060008F4 RID: 2292 RVA: 0x0001E35D File Offset: 0x0001C55D
		[ViewVariables]
		[DataField("previousOffset", false, 1, false, false, null)]
		public Vector2 PreviousOffset { get; set; } = new Vector2(0f, 0f);
	}
}
