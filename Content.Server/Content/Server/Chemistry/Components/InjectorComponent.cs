using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006A7 RID: 1703
	[RegisterComponent]
	public sealed class InjectorComponent : SharedInjectorComponent
	{
		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06002381 RID: 9089 RVA: 0x000B9A33 File Offset: 0x000B7C33
		// (set) Token: 0x06002382 RID: 9090 RVA: 0x000B9A3B File Offset: 0x000B7C3B
		[DataField("minTransferAmount", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2 MinimumTransferAmount { get; set; } = FixedPoint2.New(5);

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06002383 RID: 9091 RVA: 0x000B9A44 File Offset: 0x000B7C44
		// (set) Token: 0x06002384 RID: 9092 RVA: 0x000B9A4C File Offset: 0x000B7C4C
		[DataField("maxTransferAmount", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2 MaximumTransferAmount { get; set; } = FixedPoint2.New(50);

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06002385 RID: 9093 RVA: 0x000B9A55 File Offset: 0x000B7C55
		// (set) Token: 0x06002386 RID: 9094 RVA: 0x000B9A5D File Offset: 0x000B7C5D
		[ViewVariables]
		public SharedInjectorComponent.InjectorToggleMode ToggleState
		{
			get
			{
				return this._toggleState;
			}
			set
			{
				this._toggleState = value;
				base.Dirty(null);
			}
		}

		// Token: 0x040015E2 RID: 5602
		[Nullable(1)]
		public const string SolutionName = "injector";

		// Token: 0x040015E3 RID: 5603
		[DataField("injectOnly", false, 1, false, false, null)]
		public bool InjectOnly;

		// Token: 0x040015E4 RID: 5604
		[DataField("ignoreMobs", false, 1, false, false, null)]
		public bool IgnoreMobs;

		// Token: 0x040015E7 RID: 5607
		[ViewVariables]
		[DataField("transferAmount", false, 1, false, false, null)]
		public FixedPoint2 TransferAmount = FixedPoint2.New(5);

		// Token: 0x040015E8 RID: 5608
		[ViewVariables]
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 5f;

		// Token: 0x040015E9 RID: 5609
		[DataField("toggleState", false, 1, false, false, null)]
		private SharedInjectorComponent.InjectorToggleMode _toggleState;
	}
}
