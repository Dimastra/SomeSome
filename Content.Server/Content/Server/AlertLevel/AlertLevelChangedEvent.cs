using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.AlertLevel
{
	// Token: 0x020007E1 RID: 2017
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AlertLevelChangedEvent : EntityEventArgs
	{
		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x06002BCF RID: 11215 RVA: 0x000E5E54 File Offset: 0x000E4054
		public EntityUid Station { get; }

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x06002BD0 RID: 11216 RVA: 0x000E5E5C File Offset: 0x000E405C
		public string AlertLevel { get; }

		// Token: 0x06002BD1 RID: 11217 RVA: 0x000E5E64 File Offset: 0x000E4064
		public AlertLevelChangedEvent(EntityUid station, string alertLevel)
		{
			this.Station = station;
			this.AlertLevel = alertLevel;
		}
	}
}
