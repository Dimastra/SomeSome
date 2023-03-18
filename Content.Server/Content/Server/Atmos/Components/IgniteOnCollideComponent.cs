using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007AA RID: 1962
	[RegisterComponent]
	public sealed class IgniteOnCollideComponent : Component
	{
		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x06002A9D RID: 10909 RVA: 0x000DFF2A File Offset: 0x000DE12A
		// (set) Token: 0x06002A9E RID: 10910 RVA: 0x000DFF32 File Offset: 0x000DE132
		[DataField("fireStacks", false, 1, false, false, null)]
		public float FireStacks { get; set; }
	}
}
