using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Radio;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.GameTicking.Rules.Configurations
{
	// Token: 0x020004CB RID: 1227
	public sealed class SolarFlareEventRuleConfiguration : StationEventRuleConfiguration
	{
		// Token: 0x04001000 RID: 4096
		[DataField("minEndAfter", false, 1, false, false, null)]
		public int MinEndAfter;

		// Token: 0x04001001 RID: 4097
		[DataField("maxEndAfter", false, 1, false, false, null)]
		public int MaxEndAfter;

		// Token: 0x04001002 RID: 4098
		[DataField("onlyJamHeadsets", false, 1, false, false, null)]
		public bool OnlyJamHeadsets;

		// Token: 0x04001003 RID: 4099
		[Nullable(1)]
		[DataField("affectedChannels", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<RadioChannelPrototype>))]
		public readonly HashSet<string> AffectedChannels = new HashSet<string>();

		// Token: 0x04001004 RID: 4100
		[DataField("lightBreakChance", false, 1, false, false, null)]
		public float LightBreakChance;
	}
}
