using System;
using Content.Shared.Crayon;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Crayon
{
	// Token: 0x02000375 RID: 885
	[RegisterComponent]
	public sealed class CrayonComponent : SharedCrayonComponent
	{
		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x060015C6 RID: 5574 RVA: 0x00080F7E File Offset: 0x0007F17E
		// (set) Token: 0x060015C7 RID: 5575 RVA: 0x00080F86 File Offset: 0x0007F186
		[ViewVariables]
		public int Charges { get; set; }

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x060015C8 RID: 5576 RVA: 0x00080F8F File Offset: 0x0007F18F
		// (set) Token: 0x060015C9 RID: 5577 RVA: 0x00080F97 File Offset: 0x0007F197
		[ViewVariables]
		public int Capacity { get; set; }

		// Token: 0x04000B63 RID: 2915
		[ViewVariables]
		public bool UIUpdateNeeded;
	}
}
