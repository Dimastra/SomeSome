using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Borgs
{
	// Token: 0x0200064F RID: 1615
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class LawsComponent : Component
	{
		// Token: 0x0400137A RID: 4986
		[Nullable(1)]
		[DataField("laws", false, 1, false, false, null)]
		public List<string> Laws = new List<string>();

		// Token: 0x0400137B RID: 4987
		[DataField("canState", false, 1, false, false, null)]
		public bool CanState = true;

		// Token: 0x0400137C RID: 4988
		public TimeSpan? StateTime;

		// Token: 0x0400137D RID: 4989
		[DataField("stateCD", false, 1, false, false, null)]
		public TimeSpan StateCD = TimeSpan.FromSeconds(30.0);
	}
}
