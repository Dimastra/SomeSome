using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007AB RID: 1963
	[RegisterComponent]
	public sealed class IgniteOnMeleeHitComponent : Component
	{
		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x06002AA0 RID: 10912 RVA: 0x000DFF43 File Offset: 0x000DE143
		// (set) Token: 0x06002AA1 RID: 10913 RVA: 0x000DFF4B File Offset: 0x000DE14B
		[DataField("fireStacks", false, 1, false, false, null)]
		public float FireStacks { get; set; }
	}
}
