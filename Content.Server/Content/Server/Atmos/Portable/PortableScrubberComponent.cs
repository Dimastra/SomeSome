using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Portable
{
	// Token: 0x02000746 RID: 1862
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class PortableScrubberComponent : Component
	{
		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x0600270A RID: 9994 RVA: 0x000CD45C File Offset: 0x000CB65C
		[DataField("gasMixture", false, 1, false, false, null)]
		[ViewVariables]
		public GasMixture Air { get; } = new GasMixture();

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x0600270B RID: 9995 RVA: 0x000CD464 File Offset: 0x000CB664
		// (set) Token: 0x0600270C RID: 9996 RVA: 0x000CD46C File Offset: 0x000CB66C
		[DataField("port", false, 1, false, false, null)]
		[ViewVariables]
		public string PortName { get; set; } = "port";

		// Token: 0x04001849 RID: 6217
		[DataField("filterGases", false, 1, false, false, null)]
		public HashSet<Gas> FilterGases = new HashSet<Gas>
		{
			Gas.CarbonDioxide,
			Gas.Plasma,
			Gas.Tritium,
			Gas.WaterVapor,
			Gas.Miasma,
			Gas.NitrousOxide,
			Gas.Frezon
		};

		// Token: 0x0400184A RID: 6218
		[ViewVariables]
		public bool Enabled = true;

		// Token: 0x0400184B RID: 6219
		[ViewVariables]
		public float MaxPressure = 2500f;

		// Token: 0x0400184C RID: 6220
		[DataField("baseMaxPressure", false, 1, false, false, null)]
		public float BaseMaxPressure = 2500f;

		// Token: 0x0400184D RID: 6221
		[DataField("machinePartMaxPressure", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartMaxPressure = "MatterBin";

		// Token: 0x0400184E RID: 6222
		[DataField("partRatingMaxPressureModifier", false, 1, false, false, null)]
		public float PartRatingMaxPressureModifier = 1.5f;

		// Token: 0x0400184F RID: 6223
		[ViewVariables]
		public float TransferRate = 800f;

		// Token: 0x04001850 RID: 6224
		[DataField("baseTransferRate", false, 1, false, false, null)]
		public float BaseTransferRate = 800f;

		// Token: 0x04001851 RID: 6225
		[DataField("machinePartTransferRate", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartTransferRate = "Manipulator";

		// Token: 0x04001852 RID: 6226
		[DataField("partRatingTransferRateModifier", false, 1, false, false, null)]
		public float PartRatingTransferRateModifier = 1.4f;
	}
}
