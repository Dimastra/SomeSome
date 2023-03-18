using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.GameTicking.Prototypes
{
	// Token: 0x020004CF RID: 1231
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("lobbyBackground", 1)]
	public sealed class LobbyBackgroundPrototype : IPrototype
	{
		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06001968 RID: 6504 RVA: 0x0008612B File Offset: 0x0008432B
		// (set) Token: 0x06001969 RID: 6505 RVA: 0x00086133 File Offset: 0x00084333
		[IdDataField(1, null)]
		public string ID { get; set; }

		// Token: 0x0400101A RID: 4122
		[DataField("background", false, 1, true, false, null)]
		public ResourcePath Background;
	}
}
