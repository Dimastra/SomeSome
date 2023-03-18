using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002B2 RID: 690
	[RegisterComponent]
	public sealed class CableVisComponent : Component
	{
		// Token: 0x0400083B RID: 2107
		[Nullable(2)]
		[ViewVariables]
		[DataField("node", false, 1, false, false, null)]
		public string Node;
	}
}
