using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chemistry.Components
{
	// Token: 0x020005F9 RID: 1529
	[NetworkedComponent]
	public abstract class SharedHyposprayComponent : Component
	{
		// Token: 0x04001154 RID: 4436
		[Nullable(1)]
		[DataField("solutionName", false, 1, false, false, null)]
		public string SolutionName = "hypospray";
	}
}
