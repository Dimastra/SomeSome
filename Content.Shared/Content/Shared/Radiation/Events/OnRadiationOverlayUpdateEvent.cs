using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Radiation.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Events
{
	// Token: 0x0200022A RID: 554
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class OnRadiationOverlayUpdateEvent : EntityEventArgs
	{
		// Token: 0x0600062C RID: 1580 RVA: 0x00015C8F File Offset: 0x00013E8F
		public OnRadiationOverlayUpdateEvent(double elapsedTimeMs, int sourcesCount, int receiversCount, List<RadiationRay> rays)
		{
			this.ElapsedTimeMs = elapsedTimeMs;
			this.SourcesCount = sourcesCount;
			this.ReceiversCount = receiversCount;
			this.Rays = rays;
		}

		// Token: 0x0400062B RID: 1579
		public readonly double ElapsedTimeMs;

		// Token: 0x0400062C RID: 1580
		public readonly int SourcesCount;

		// Token: 0x0400062D RID: 1581
		public readonly int ReceiversCount;

		// Token: 0x0400062E RID: 1582
		public readonly List<RadiationRay> Rays;
	}
}
