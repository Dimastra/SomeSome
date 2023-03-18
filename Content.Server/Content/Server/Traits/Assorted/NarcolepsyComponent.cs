using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Traits.Assorted
{
	// Token: 0x02000103 RID: 259
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(NarcolepsySystem)
	})]
	public sealed class NarcolepsyComponent : Component
	{
		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x000168E4 File Offset: 0x00014AE4
		[DataField("timeBetweenIncidents", false, 1, true, false, null)]
		public Vector2 TimeBetweenIncidents { get; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060004B5 RID: 1205 RVA: 0x000168EC File Offset: 0x00014AEC
		[DataField("durationOfIncident", false, 1, true, false, null)]
		public Vector2 DurationOfIncident { get; }

		// Token: 0x040002BC RID: 700
		public float NextIncidentTime;
	}
}
