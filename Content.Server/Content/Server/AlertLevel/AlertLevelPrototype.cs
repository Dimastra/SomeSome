using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.AlertLevel
{
	// Token: 0x020007DC RID: 2012
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("alertLevels", 1)]
	public sealed class AlertLevelPrototype : IPrototype
	{
		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x06002BB9 RID: 11193 RVA: 0x000E5983 File Offset: 0x000E3B83
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x06002BBA RID: 11194 RVA: 0x000E598B File Offset: 0x000E3B8B
		[DataField("defaultLevel", false, 1, false, false, null)]
		public string DefaultLevel { get; }

		// Token: 0x04001B28 RID: 6952
		[DataField("levels", false, 1, false, false, null)]
		public Dictionary<string, AlertLevelDetail> Levels = new Dictionary<string, AlertLevelDetail>();
	}
}
