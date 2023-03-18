using System;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006B3 RID: 1715
	[RegisterComponent]
	public sealed class VaporComponent : Component
	{
		// Token: 0x0400161C RID: 5660
		[Nullable(1)]
		public const string SolutionName = "vapor";

		// Token: 0x0400161D RID: 5661
		[DataField("transferAmount", false, 1, false, false, null)]
		public FixedPoint2 TransferAmount = FixedPoint2.New(0.5);

		// Token: 0x0400161E RID: 5662
		public float ReactTimer;

		// Token: 0x0400161F RID: 5663
		public bool Active;
	}
}
