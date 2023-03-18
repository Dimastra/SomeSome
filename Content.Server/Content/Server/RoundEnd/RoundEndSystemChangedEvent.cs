using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.RoundEnd
{
	// Token: 0x02000225 RID: 549
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RoundEndSystemChangedEvent : EntityEventArgs
	{
		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000AFC RID: 2812 RVA: 0x00039C5A File Offset: 0x00037E5A
		public static RoundEndSystemChangedEvent Default { get; } = new RoundEndSystemChangedEvent();
	}
}
