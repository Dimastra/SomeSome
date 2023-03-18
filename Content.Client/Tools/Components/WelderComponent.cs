using System;
using Content.Client.Tools.UI;
using Content.Shared.Tools.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Tools.Components
{
	// Token: 0x020000F3 RID: 243
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ToolSystem),
		typeof(WelderStatusControl)
	})]
	public sealed class WelderComponent : SharedWelderComponent
	{
		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x0002409B File Offset: 0x0002229B
		// (set) Token: 0x060006DD RID: 1757 RVA: 0x000240A3 File Offset: 0x000222A3
		[ViewVariables]
		public bool UiUpdateNeeded { get; set; }

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x000240AC File Offset: 0x000222AC
		// (set) Token: 0x060006DF RID: 1759 RVA: 0x000240B4 File Offset: 0x000222B4
		[ViewVariables]
		public float FuelCapacity { get; set; }

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x000240BD File Offset: 0x000222BD
		// (set) Token: 0x060006E1 RID: 1761 RVA: 0x000240C5 File Offset: 0x000222C5
		[ViewVariables]
		public float Fuel { get; set; }
	}
}
