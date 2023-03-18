using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Kudzu
{
	// Token: 0x0200028D RID: 653
	[RegisterComponent]
	public sealed class KudzuVisualsComponent : Component
	{
		// Token: 0x17000394 RID: 916
		// (get) Token: 0x060010A0 RID: 4256 RVA: 0x00063800 File Offset: 0x00061A00
		[DataField("layer", false, 1, false, false, null)]
		public int Layer { get; }
	}
}
