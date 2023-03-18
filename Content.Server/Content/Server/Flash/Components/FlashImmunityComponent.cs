using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Flash.Components
{
	// Token: 0x020004FD RID: 1277
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(FlashSystem)
	})]
	public sealed class FlashImmunityComponent : Component
	{
		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06001A4B RID: 6731 RVA: 0x0008AB52 File Offset: 0x00088D52
		// (set) Token: 0x06001A4C RID: 6732 RVA: 0x0008AB5A File Offset: 0x00088D5A
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled { get; set; } = true;
	}
}
