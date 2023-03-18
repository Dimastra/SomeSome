using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005F0 RID: 1520
	[DataDefinition]
	public sealed class ReactantPrototype
	{
		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06001277 RID: 4727 RVA: 0x0003C28E File Offset: 0x0003A48E
		public FixedPoint2 Amount
		{
			get
			{
				return this._amount;
			}
		}

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06001278 RID: 4728 RVA: 0x0003C296 File Offset: 0x0003A496
		public bool Catalyst
		{
			get
			{
				return this._catalyst;
			}
		}

		// Token: 0x0400113E RID: 4414
		[DataField("amount", false, 1, false, false, null)]
		private FixedPoint2 _amount = FixedPoint2.New(1);

		// Token: 0x0400113F RID: 4415
		[DataField("catalyst", false, 1, false, false, null)]
		private bool _catalyst;
	}
}
