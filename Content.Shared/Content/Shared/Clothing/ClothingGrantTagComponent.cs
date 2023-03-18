using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Clothing
{
	// Token: 0x020005A9 RID: 1449
	[RegisterComponent]
	public sealed class ClothingGrantTagComponent : Component
	{
		// Token: 0x04001052 RID: 4178
		[Nullable(1)]
		[DataField("tag", false, 1, true, false, null)]
		[ViewVariables]
		public string Tag = "";

		// Token: 0x04001053 RID: 4179
		[ViewVariables]
		public bool IsActive;
	}
}
