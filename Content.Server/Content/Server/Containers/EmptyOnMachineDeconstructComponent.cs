using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Containers
{
	// Token: 0x020005EB RID: 1515
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class EmptyOnMachineDeconstructComponent : Component
	{
		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x0600204B RID: 8267 RVA: 0x000A8644 File Offset: 0x000A6844
		// (set) Token: 0x0600204C RID: 8268 RVA: 0x000A864C File Offset: 0x000A684C
		[DataField("containers", false, 1, false, false, null)]
		public HashSet<string> Containers { get; set; } = new HashSet<string>();
	}
}
