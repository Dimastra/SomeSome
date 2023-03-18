using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.White.StationGoal
{
	// Token: 0x0200008B RID: 139
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("stationGoal", 1)]
	[Serializable]
	public sealed class StationGoalPrototype : IPrototype
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000213 RID: 531 RVA: 0x0000BE30 File Offset: 0x0000A030
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000214 RID: 532 RVA: 0x0000BE38 File Offset: 0x0000A038
		// (set) Token: 0x06000215 RID: 533 RVA: 0x0000BE40 File Offset: 0x0000A040
		[DataField("text", false, 1, false, false, null)]
		public string Text { get; set; } = string.Empty;
	}
}
