using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Pow3r;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002B9 RID: 697
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class PowerNetworkBatteryComponent : Component
	{
		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000E05 RID: 3589 RVA: 0x00047C72 File Offset: 0x00045E72
		// (set) Token: 0x06000E06 RID: 3590 RVA: 0x00047C7F File Offset: 0x00045E7F
		[DataField("maxChargeRate", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxChargeRate
		{
			get
			{
				return this.NetworkBattery.MaxChargeRate;
			}
			set
			{
				this.NetworkBattery.MaxChargeRate = value;
			}
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000E07 RID: 3591 RVA: 0x00047C8D File Offset: 0x00045E8D
		// (set) Token: 0x06000E08 RID: 3592 RVA: 0x00047C9A File Offset: 0x00045E9A
		[DataField("maxSupply", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxSupply
		{
			get
			{
				return this.NetworkBattery.MaxSupply;
			}
			set
			{
				this.NetworkBattery.MaxSupply = value;
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000E09 RID: 3593 RVA: 0x00047CA8 File Offset: 0x00045EA8
		// (set) Token: 0x06000E0A RID: 3594 RVA: 0x00047CB5 File Offset: 0x00045EB5
		[DataField("supplyRampTolerance", false, 1, false, false, null)]
		[ViewVariables]
		public float SupplyRampTolerance
		{
			get
			{
				return this.NetworkBattery.SupplyRampTolerance;
			}
			set
			{
				this.NetworkBattery.SupplyRampTolerance = value;
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000E0B RID: 3595 RVA: 0x00047CC3 File Offset: 0x00045EC3
		// (set) Token: 0x06000E0C RID: 3596 RVA: 0x00047CD0 File Offset: 0x00045ED0
		[DataField("supplyRampRate", false, 1, false, false, null)]
		[ViewVariables]
		public float SupplyRampRate
		{
			get
			{
				return this.NetworkBattery.SupplyRampRate;
			}
			set
			{
				this.NetworkBattery.SupplyRampRate = value;
			}
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000E0D RID: 3597 RVA: 0x00047CDE File Offset: 0x00045EDE
		// (set) Token: 0x06000E0E RID: 3598 RVA: 0x00047CEB File Offset: 0x00045EEB
		[DataField("supplyRampPosition", false, 1, false, false, null)]
		[ViewVariables]
		public float SupplyRampPosition
		{
			get
			{
				return this.NetworkBattery.SupplyRampPosition;
			}
			set
			{
				this.NetworkBattery.SupplyRampPosition = value;
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000E0F RID: 3599 RVA: 0x00047CF9 File Offset: 0x00045EF9
		// (set) Token: 0x06000E10 RID: 3600 RVA: 0x00047D06 File Offset: 0x00045F06
		[DataField("currentSupply", false, 1, false, false, null)]
		[ViewVariables]
		public float CurrentSupply
		{
			get
			{
				return this.NetworkBattery.CurrentSupply;
			}
			set
			{
				this.NetworkBattery.CurrentSupply = value;
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000E11 RID: 3601 RVA: 0x00047D14 File Offset: 0x00045F14
		// (set) Token: 0x06000E12 RID: 3602 RVA: 0x00047D21 File Offset: 0x00045F21
		[DataField("currentReceiving", false, 1, false, false, null)]
		[ViewVariables]
		public float CurrentReceiving
		{
			get
			{
				return this.NetworkBattery.CurrentReceiving;
			}
			set
			{
				this.NetworkBattery.CurrentReceiving = value;
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000E13 RID: 3603 RVA: 0x00047D2F File Offset: 0x00045F2F
		// (set) Token: 0x06000E14 RID: 3604 RVA: 0x00047D3C File Offset: 0x00045F3C
		[DataField("loadingNetworkDemand", false, 1, false, false, null)]
		[ViewVariables]
		public float LoadingNetworkDemand
		{
			get
			{
				return this.NetworkBattery.LoadingNetworkDemand;
			}
			set
			{
				this.NetworkBattery.LoadingNetworkDemand = value;
			}
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000E15 RID: 3605 RVA: 0x00047D4A File Offset: 0x00045F4A
		// (set) Token: 0x06000E16 RID: 3606 RVA: 0x00047D57 File Offset: 0x00045F57
		[DataField("enabled", false, 1, false, false, null)]
		[ViewVariables]
		public bool Enabled
		{
			get
			{
				return this.NetworkBattery.Enabled;
			}
			set
			{
				this.NetworkBattery.Enabled = value;
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000E17 RID: 3607 RVA: 0x00047D65 File Offset: 0x00045F65
		// (set) Token: 0x06000E18 RID: 3608 RVA: 0x00047D72 File Offset: 0x00045F72
		[DataField("canCharge", false, 1, false, false, null)]
		[ViewVariables]
		public bool CanCharge
		{
			get
			{
				return this.NetworkBattery.CanCharge;
			}
			set
			{
				this.NetworkBattery.CanCharge = value;
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000E19 RID: 3609 RVA: 0x00047D80 File Offset: 0x00045F80
		// (set) Token: 0x06000E1A RID: 3610 RVA: 0x00047D8D File Offset: 0x00045F8D
		[DataField("canDisharge", false, 1, false, false, null)]
		[ViewVariables]
		public bool CanDischarge
		{
			get
			{
				return this.NetworkBattery.CanDischarge;
			}
			set
			{
				this.NetworkBattery.CanDischarge = value;
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06000E1B RID: 3611 RVA: 0x00047D9B File Offset: 0x00045F9B
		// (set) Token: 0x06000E1C RID: 3612 RVA: 0x00047DA8 File Offset: 0x00045FA8
		[DataField("efficiency", false, 1, false, false, null)]
		[ViewVariables]
		public float Efficiency
		{
			get
			{
				return this.NetworkBattery.Efficiency;
			}
			set
			{
				this.NetworkBattery.Efficiency = value;
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06000E1D RID: 3613 RVA: 0x00047DB6 File Offset: 0x00045FB6
		[ViewVariables]
		public PowerState.Battery NetworkBattery { get; } = new PowerState.Battery();

		// Token: 0x04000847 RID: 2119
		[ViewVariables]
		public float LastSupply;
	}
}
