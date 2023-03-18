using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.NodeGroups;
using Content.Server.Power.Pow3r;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002BA RID: 698
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class PowerSupplierComponent : BasePowerNetComponent
	{
		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06000E1F RID: 3615 RVA: 0x00047DD1 File Offset: 0x00045FD1
		// (set) Token: 0x06000E20 RID: 3616 RVA: 0x00047DDE File Offset: 0x00045FDE
		[ViewVariables]
		[DataField("supplyRate", false, 1, false, false, null)]
		public float MaxSupply
		{
			get
			{
				return this.NetworkSupply.MaxSupply;
			}
			set
			{
				this.NetworkSupply.MaxSupply = value;
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000E21 RID: 3617 RVA: 0x00047DEC File Offset: 0x00045FEC
		// (set) Token: 0x06000E22 RID: 3618 RVA: 0x00047DF9 File Offset: 0x00045FF9
		[ViewVariables]
		[DataField("supplyRampTolerance", false, 1, false, false, null)]
		public float SupplyRampTolerance
		{
			get
			{
				return this.NetworkSupply.SupplyRampTolerance;
			}
			set
			{
				this.NetworkSupply.SupplyRampTolerance = value;
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000E23 RID: 3619 RVA: 0x00047E07 File Offset: 0x00046007
		// (set) Token: 0x06000E24 RID: 3620 RVA: 0x00047E14 File Offset: 0x00046014
		[ViewVariables]
		[DataField("supplyRampRate", false, 1, false, false, null)]
		public float SupplyRampRate
		{
			get
			{
				return this.NetworkSupply.SupplyRampRate;
			}
			set
			{
				this.NetworkSupply.SupplyRampRate = value;
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06000E25 RID: 3621 RVA: 0x00047E22 File Offset: 0x00046022
		// (set) Token: 0x06000E26 RID: 3622 RVA: 0x00047E2F File Offset: 0x0004602F
		[ViewVariables]
		[DataField("supplyRampPosition", false, 1, false, false, null)]
		public float SupplyRampPosition
		{
			get
			{
				return this.NetworkSupply.SupplyRampPosition;
			}
			set
			{
				this.NetworkSupply.SupplyRampPosition = value;
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06000E27 RID: 3623 RVA: 0x00047E3D File Offset: 0x0004603D
		// (set) Token: 0x06000E28 RID: 3624 RVA: 0x00047E4A File Offset: 0x0004604A
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled
		{
			get
			{
				return this.NetworkSupply.Enabled;
			}
			set
			{
				this.NetworkSupply.Enabled = value;
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06000E29 RID: 3625 RVA: 0x00047E58 File Offset: 0x00046058
		[ViewVariables]
		public float CurrentSupply
		{
			get
			{
				return this.NetworkSupply.CurrentSupply;
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06000E2A RID: 3626 RVA: 0x00047E65 File Offset: 0x00046065
		[ViewVariables]
		public PowerState.Supply NetworkSupply { get; } = new PowerState.Supply();

		// Token: 0x06000E2B RID: 3627 RVA: 0x00047E6D File Offset: 0x0004606D
		protected override void AddSelfToNet(IPowerNet powerNet)
		{
			powerNet.AddSupplier(this);
		}

		// Token: 0x06000E2C RID: 3628 RVA: 0x00047E76 File Offset: 0x00046076
		protected override void RemoveSelfFromNet(IPowerNet powerNet)
		{
			powerNet.RemoveSupplier(this);
		}
	}
}
