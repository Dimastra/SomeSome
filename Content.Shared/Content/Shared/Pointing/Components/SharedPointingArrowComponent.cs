using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Pointing.Components
{
	// Token: 0x02000267 RID: 615
	[NetworkedComponent]
	public abstract class SharedPointingArrowComponent : Component
	{
		// Token: 0x040006F1 RID: 1777
		[ViewVariables]
		[DataField("endTime", false, 1, false, false, null)]
		public TimeSpan EndTime;
	}
}
