using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Components
{
	// Token: 0x020005FD RID: 1533
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class SolutionTransferComponent : Component
	{
		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x060012C9 RID: 4809 RVA: 0x0003DB0C File Offset: 0x0003BD0C
		// (set) Token: 0x060012CA RID: 4810 RVA: 0x0003DB14 File Offset: 0x0003BD14
		[DataField("transferAmount", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2 TransferAmount { get; set; } = FixedPoint2.New(5);

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x060012CB RID: 4811 RVA: 0x0003DB1D File Offset: 0x0003BD1D
		// (set) Token: 0x060012CC RID: 4812 RVA: 0x0003DB25 File Offset: 0x0003BD25
		[DataField("minTransferAmount", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2 MinimumTransferAmount { get; set; } = FixedPoint2.New(5);

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x060012CD RID: 4813 RVA: 0x0003DB2E File Offset: 0x0003BD2E
		// (set) Token: 0x060012CE RID: 4814 RVA: 0x0003DB36 File Offset: 0x0003BD36
		[DataField("maxTransferAmount", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2 MaximumTransferAmount { get; set; } = FixedPoint2.New(50);

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x060012CF RID: 4815 RVA: 0x0003DB3F File Offset: 0x0003BD3F
		// (set) Token: 0x060012D0 RID: 4816 RVA: 0x0003DB47 File Offset: 0x0003BD47
		[DataField("canReceive", false, 1, false, false, null)]
		[ViewVariables]
		public bool CanReceive { get; set; } = true;

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x060012D1 RID: 4817 RVA: 0x0003DB50 File Offset: 0x0003BD50
		// (set) Token: 0x060012D2 RID: 4818 RVA: 0x0003DB58 File Offset: 0x0003BD58
		[DataField("canSend", false, 1, false, false, null)]
		[ViewVariables]
		public bool CanSend { get; set; } = true;

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x060012D3 RID: 4819 RVA: 0x0003DB61 File Offset: 0x0003BD61
		// (set) Token: 0x060012D4 RID: 4820 RVA: 0x0003DB69 File Offset: 0x0003BD69
		[DataField("canChangeTransferAmount", false, 1, false, false, null)]
		[ViewVariables]
		public bool CanChangeTransferAmount { get; set; }
	}
}
