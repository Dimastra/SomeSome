using System;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Body.Components
{
	// Token: 0x02000716 RID: 1814
	[DataDefinition]
	public sealed class MetabolismGroupEntry
	{
		// Token: 0x040017AB RID: 6059
		[Nullable(1)]
		[DataField("id", false, 1, true, false, typeof(PrototypeIdSerializer<MetabolismGroupPrototype>))]
		public string Id;

		// Token: 0x040017AC RID: 6060
		[DataField("rateModifier", false, 1, false, false, null)]
		public FixedPoint2 MetabolismRateModifier = 1.0;
	}
}
