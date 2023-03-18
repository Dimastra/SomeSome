using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Research.Components
{
	// Token: 0x02000204 RID: 516
	[RegisterComponent]
	public sealed class ResearchClientComponent : Component
	{
		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x00014AD4 File Offset: 0x00012CD4
		public bool ConnectedToServer
		{
			get
			{
				return this.Server != null;
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060005BD RID: 1469 RVA: 0x00014AEF File Offset: 0x00012CEF
		// (set) Token: 0x060005BE RID: 1470 RVA: 0x00014AF7 File Offset: 0x00012CF7
		[ViewVariables]
		public EntityUid? Server { get; set; }
	}
}
