using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Traits.Assorted
{
	// Token: 0x02000107 RID: 263
	[RegisterComponent]
	public sealed class UncontrollableSnoughComponent : Component
	{
		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060004BF RID: 1215 RVA: 0x00016AB0 File Offset: 0x00014CB0
		[DataField("timeBetweenIncidents", false, 1, true, false, null)]
		public Vector2 TimeBetweenIncidents { get; }

		// Token: 0x040002C0 RID: 704
		[Nullable(1)]
		[DataField("snoughMessage", false, 1, false, false, null)]
		public string SnoughMessage = "disease-sneeze";

		// Token: 0x040002C1 RID: 705
		[Nullable(2)]
		[DataField("snoughSound", false, 1, false, false, null)]
		public SoundSpecifier SnoughSound;

		// Token: 0x040002C3 RID: 707
		public float NextIncidentTime;
	}
}
