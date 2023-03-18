using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Piping.Binary.Components;
using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Unary.Components
{
	// Token: 0x02000750 RID: 1872
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(GasOutletInjectorSystem)
	})]
	public sealed class GasOutletInjectorComponent : Component
	{
		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x0600276D RID: 10093 RVA: 0x000CFAF2 File Offset: 0x000CDCF2
		// (set) Token: 0x0600276E RID: 10094 RVA: 0x000CFAFA File Offset: 0x000CDCFA
		[ViewVariables]
		public bool Enabled { get; set; } = true;

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x0600276F RID: 10095 RVA: 0x000CFB03 File Offset: 0x000CDD03
		// (set) Token: 0x06002770 RID: 10096 RVA: 0x000CFB0B File Offset: 0x000CDD0B
		[ViewVariables]
		public float TransferRate
		{
			get
			{
				return this._transferRate;
			}
			set
			{
				this._transferRate = Math.Clamp(value, 0f, this.MaxTransferRate);
			}
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x06002771 RID: 10097 RVA: 0x000CFB24 File Offset: 0x000CDD24
		// (set) Token: 0x06002772 RID: 10098 RVA: 0x000CFB2C File Offset: 0x000CDD2C
		[DataField("maxPressure", false, 1, false, false, null)]
		public float MaxPressure { get; set; } = GasVolumePumpComponent.DefaultHigherThreshold;

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06002773 RID: 10099 RVA: 0x000CFB35 File Offset: 0x000CDD35
		// (set) Token: 0x06002774 RID: 10100 RVA: 0x000CFB3D File Offset: 0x000CDD3D
		[DataField("inlet", false, 1, false, false, null)]
		public string InletName { get; set; } = "pipe";

		// Token: 0x04001885 RID: 6277
		private float _transferRate = 50f;

		// Token: 0x04001886 RID: 6278
		[ViewVariables]
		[DataField("maxTransferRate", false, 1, false, false, null)]
		public float MaxTransferRate = 200f;
	}
}
