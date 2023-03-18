using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Unary.Components
{
	// Token: 0x0200074F RID: 1871
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasCanisterComponent : Component, IGasMixtureHolder
	{
		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x0600275C RID: 10076 RVA: 0x000CF9FB File Offset: 0x000CDBFB
		// (set) Token: 0x0600275D RID: 10077 RVA: 0x000CFA03 File Offset: 0x000CDC03
		[ViewVariables]
		[DataField("port", false, 1, false, false, null)]
		public string PortName { get; set; } = "port";

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x0600275E RID: 10078 RVA: 0x000CFA0C File Offset: 0x000CDC0C
		// (set) Token: 0x0600275F RID: 10079 RVA: 0x000CFA14 File Offset: 0x000CDC14
		[ViewVariables]
		[DataField("container", false, 1, false, false, null)]
		public string ContainerName { get; set; } = "GasCanisterTankHolder";

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06002760 RID: 10080 RVA: 0x000CFA1D File Offset: 0x000CDC1D
		// (set) Token: 0x06002761 RID: 10081 RVA: 0x000CFA25 File Offset: 0x000CDC25
		[ViewVariables]
		[DataField("gasMixture", false, 1, false, false, null)]
		public GasMixture Air { get; set; } = new GasMixture();

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06002762 RID: 10082 RVA: 0x000CFA2E File Offset: 0x000CDC2E
		// (set) Token: 0x06002763 RID: 10083 RVA: 0x000CFA36 File Offset: 0x000CDC36
		public float LastPressure { get; set; }

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x06002764 RID: 10084 RVA: 0x000CFA3F File Offset: 0x000CDC3F
		// (set) Token: 0x06002765 RID: 10085 RVA: 0x000CFA47 File Offset: 0x000CDC47
		[ViewVariables]
		[DataField("minReleasePressure", false, 1, false, false, null)]
		public float MinReleasePressure { get; set; } = 10.1325f;

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x06002766 RID: 10086 RVA: 0x000CFA50 File Offset: 0x000CDC50
		// (set) Token: 0x06002767 RID: 10087 RVA: 0x000CFA58 File Offset: 0x000CDC58
		[ViewVariables]
		[DataField("maxReleasePressure", false, 1, false, false, null)]
		public float MaxReleasePressure { get; set; } = 1013.25f;

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06002768 RID: 10088 RVA: 0x000CFA61 File Offset: 0x000CDC61
		// (set) Token: 0x06002769 RID: 10089 RVA: 0x000CFA69 File Offset: 0x000CDC69
		[ViewVariables]
		[DataField("releasePressure", false, 1, false, false, null)]
		public float ReleasePressure { get; set; } = 101.325f;

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x0600276A RID: 10090 RVA: 0x000CFA72 File Offset: 0x000CDC72
		// (set) Token: 0x0600276B RID: 10091 RVA: 0x000CFA7A File Offset: 0x000CDC7A
		[ViewVariables]
		[DataField("releaseValve", false, 1, false, false, null)]
		public bool ReleaseValve { get; set; }

		// Token: 0x04001883 RID: 6275
		[DataField("accessDeniedSound", false, 1, false, false, null)]
		public SoundSpecifier AccessDeniedSound = new SoundPathSpecifier("/Audio/Machines/custom_deny.ogg", null);
	}
}
