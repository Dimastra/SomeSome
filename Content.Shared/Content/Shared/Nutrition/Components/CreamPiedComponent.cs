using System;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Nutrition.Components
{
	// Token: 0x020002B0 RID: 688
	[Access(new Type[]
	{
		typeof(SharedCreamPieSystem)
	})]
	[RegisterComponent]
	public sealed class CreamPiedComponent : Component
	{
		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060007BA RID: 1978 RVA: 0x00019F87 File Offset: 0x00018187
		// (set) Token: 0x060007BB RID: 1979 RVA: 0x00019F8F File Offset: 0x0001818F
		[ViewVariables]
		public bool CreamPied { get; set; }
	}
}
