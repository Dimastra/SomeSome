using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Mousetrap
{
	// Token: 0x02000394 RID: 916
	[RegisterComponent]
	public sealed class MousetrapComponent : Component
	{
		// Token: 0x04000B79 RID: 2937
		[ViewVariables]
		public bool IsActive;

		// Token: 0x04000B7A RID: 2938
		[ViewVariables]
		[DataField("massBalance", false, 1, false, false, null)]
		public int MassBalance = 10;
	}
}
