using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Server.Shuttles.Components
{
	// Token: 0x02000209 RID: 521
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ShuttleComponent : Component
	{
		// Token: 0x04000651 RID: 1617
		[ViewVariables]
		public bool Enabled = true;

		// Token: 0x04000652 RID: 1618
		[ViewVariables]
		public readonly float[] LinearThrust = new float[4];

		// Token: 0x04000653 RID: 1619
		public readonly List<ThrusterComponent>[] LinearThrusters = new List<ThrusterComponent>[4];

		// Token: 0x04000654 RID: 1620
		public readonly List<ThrusterComponent> AngularThrusters = new List<ThrusterComponent>();

		// Token: 0x04000655 RID: 1621
		[ViewVariables]
		public float AngularThrust;

		// Token: 0x04000656 RID: 1622
		[ViewVariables]
		public DirectionFlag ThrustDirections;
	}
}
