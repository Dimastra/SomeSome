using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Pointing.Components
{
	// Token: 0x020002CD RID: 717
	[RegisterComponent]
	public sealed class PointingArrowAngeringComponent : Component
	{
		// Token: 0x0400088B RID: 2187
		[ViewVariables]
		[DataField("remainingAnger", false, 1, false, false, null)]
		public int RemainingAnger = 5;
	}
}
