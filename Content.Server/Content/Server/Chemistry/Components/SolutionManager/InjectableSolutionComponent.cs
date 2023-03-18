using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components.SolutionManager
{
	// Token: 0x020006B7 RID: 1719
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class InjectableSolutionComponent : Component
	{
		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x060023C5 RID: 9157 RVA: 0x000BA66F File Offset: 0x000B886F
		// (set) Token: 0x060023C6 RID: 9158 RVA: 0x000BA677 File Offset: 0x000B8877
		[ViewVariables]
		[DataField("solution", false, 1, false, false, null)]
		public string Solution { get; set; } = "default";
	}
}
