using System;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006B1 RID: 1713
	[RegisterComponent]
	internal sealed class SolutionInjectOnCollideComponent : Component
	{
		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x060023B1 RID: 9137 RVA: 0x000BA546 File Offset: 0x000B8746
		// (set) Token: 0x060023B2 RID: 9138 RVA: 0x000BA54E File Offset: 0x000B874E
		[ViewVariables]
		[DataField("transferAmount", false, 1, false, false, null)]
		public FixedPoint2 TransferAmount { get; set; } = FixedPoint2.New(1);

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x060023B3 RID: 9139 RVA: 0x000BA557 File Offset: 0x000B8757
		// (set) Token: 0x060023B4 RID: 9140 RVA: 0x000BA55F File Offset: 0x000B875F
		[ViewVariables]
		public float TransferEfficiency
		{
			get
			{
				return this._transferEfficiency;
			}
			set
			{
				this._transferEfficiency = Math.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x04001616 RID: 5654
		[DataField("transferEfficiency", false, 1, false, false, null)]
		private float _transferEfficiency = 1f;

		// Token: 0x04001617 RID: 5655
		[DataField("blockSlots", false, 1, false, false, null)]
		[ViewVariables]
		public SlotFlags BlockSlots = SlotFlags.MASK;
	}
}
