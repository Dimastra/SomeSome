using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Medical.SuitSensor
{
	// Token: 0x0200030A RID: 778
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SuitSensorStatus
	{
		// Token: 0x060008F2 RID: 2290 RVA: 0x0001E33F File Offset: 0x0001C53F
		public SuitSensorStatus(string name, string job)
		{
			this.Name = name;
			this.Job = job;
		}

		// Token: 0x040008DA RID: 2266
		public TimeSpan Timestamp;

		// Token: 0x040008DB RID: 2267
		public string Name;

		// Token: 0x040008DC RID: 2268
		public string Job;

		// Token: 0x040008DD RID: 2269
		public bool IsAlive;

		// Token: 0x040008DE RID: 2270
		public int? TotalDamage;

		// Token: 0x040008DF RID: 2271
		public EntityCoordinates? Coordinates;
	}
}
