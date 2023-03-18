using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.DetailExaminable
{
	// Token: 0x02000593 RID: 1427
	[RegisterComponent]
	public sealed class DetailExaminableComponent : Component
	{
		// Token: 0x0400131C RID: 4892
		[Nullable(1)]
		[DataField("content", false, 1, true, false, null)]
		[ViewVariables]
		public string Content = "";
	}
}
