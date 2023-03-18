using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Cleanable
{
	// Token: 0x0200064A RID: 1610
	[RegisterComponent]
	public sealed class CleanableComponent : Component
	{
		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x0600222C RID: 8748 RVA: 0x000B2E30 File Offset: 0x000B1030
		[ViewVariables]
		public FixedPoint2 CleanAmount
		{
			get
			{
				return this._cleanAmount;
			}
		}

		// Token: 0x0400151A RID: 5402
		[DataField("cleanAmount", false, 1, false, false, null)]
		private FixedPoint2 _cleanAmount = FixedPoint2.Zero;
	}
}
