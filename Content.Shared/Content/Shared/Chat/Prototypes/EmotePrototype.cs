using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chat.Prototypes
{
	// Token: 0x0200060C RID: 1548
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("emote", 1)]
	public sealed class EmotePrototype : IPrototype
	{
		// Token: 0x170003CF RID: 975
		// (get) Token: 0x060012ED RID: 4845 RVA: 0x0003E148 File Offset: 0x0003C348
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x060012EE RID: 4846 RVA: 0x0003E150 File Offset: 0x0003C350
		[DataField("buttonText", false, 1, false, false, null)]
		public string ButtonText { get; } = "Unknown";

		// Token: 0x040011AF RID: 4527
		[DataField("category", false, 1, false, false, null)]
		public EmoteCategory Category = EmoteCategory.General;

		// Token: 0x040011B1 RID: 4529
		[DataField("chatMessages", false, 1, false, false, null)]
		public List<string> ChatMessages = new List<string>();

		// Token: 0x040011B2 RID: 4530
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("chatTriggers", false, 1, false, false, null)]
		public HashSet<string> ChatTriggers = new HashSet<string>();
	}
}
