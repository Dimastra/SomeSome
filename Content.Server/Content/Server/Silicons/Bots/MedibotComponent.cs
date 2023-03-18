using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Silicons.Bots
{
	// Token: 0x020001F4 RID: 500
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class MedibotComponent : Component
	{
		// Token: 0x040005D1 RID: 1489
		[DataField("standardMed", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
		public string StandardMed = "Tricordrazine";

		// Token: 0x040005D2 RID: 1490
		[DataField("standardMedInjectAmount", false, 1, false, false, null)]
		public float StandardMedInjectAmount = 15f;

		// Token: 0x040005D3 RID: 1491
		public const float StandardMedDamageThreshold = 50f;

		// Token: 0x040005D4 RID: 1492
		[DataField("emergencyMed", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
		public string EmergencyMed = "Inaprovaline";

		// Token: 0x040005D5 RID: 1493
		[DataField("emergencyMedInjectAmount", false, 1, false, false, null)]
		public float EmergencyMedInjectAmount = 15f;

		// Token: 0x040005D6 RID: 1494
		public const float EmergencyMedDamageThreshold = 100f;
	}
}
