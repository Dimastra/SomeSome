using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Systems
{
	// Token: 0x02000226 RID: 550
	[NetSerializable]
	[Serializable]
	public sealed class RadiationRay
	{
		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000621 RID: 1569 RVA: 0x00015ABB File Offset: 0x00013CBB
		public bool ReachedDestination
		{
			get
			{
				return this.Rads > 0f;
			}
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x00015ACA File Offset: 0x00013CCA
		public RadiationRay(MapId mapId, EntityUid sourceUid, Vector2 source, EntityUid destinationUid, Vector2 destination, float rads)
		{
			this.MapId = mapId;
			this.SourceUid = sourceUid;
			this.Source = source;
			this.DestinationUid = destinationUid;
			this.Destination = destination;
			this.Rads = rads;
		}

		// Token: 0x04000621 RID: 1569
		public MapId MapId;

		// Token: 0x04000622 RID: 1570
		public EntityUid SourceUid;

		// Token: 0x04000623 RID: 1571
		public Vector2 Source;

		// Token: 0x04000624 RID: 1572
		public EntityUid DestinationUid;

		// Token: 0x04000625 RID: 1573
		public Vector2 Destination;

		// Token: 0x04000626 RID: 1574
		public float Rads;

		// Token: 0x04000627 RID: 1575
		[Nullable(new byte[]
		{
			1,
			1,
			0
		})]
		public Dictionary<EntityUid, List<ValueTuple<Vector2i, float>>> Blockers = new Dictionary<EntityUid, List<ValueTuple<Vector2i, float>>>();
	}
}
