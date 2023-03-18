using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Anomaly.Effects.Components
{
	// Token: 0x02000707 RID: 1799
	[RegisterComponent]
	public sealed class ElectricityAnomalyComponent : Component
	{
		// Token: 0x040015E5 RID: 5605
		[DataField("maxElectrocutionRange", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxElectrocuteRange = 7f;

		// Token: 0x040015E6 RID: 5606
		[DataField("maxElectrocuteDamage", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxElectrocuteDamage = 20f;

		// Token: 0x040015E7 RID: 5607
		[DataField("maxElectrocuteDuration", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan MaxElectrocuteDuration = TimeSpan.FromSeconds(8.0);

		// Token: 0x040015E8 RID: 5608
		[DataField("passiveElectrocutionChance", false, 1, false, false, null)]
		[ViewVariables]
		public float PassiveElectrocutionChance = 0.05f;

		// Token: 0x040015E9 RID: 5609
		[DataField("nextSecond", false, 1, false, false, typeof(TimeOffsetSerializer))]
		[ViewVariables]
		public TimeSpan NextSecond = TimeSpan.Zero;
	}
}
