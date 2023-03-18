using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Cloning.Components
{
	// Token: 0x02000644 RID: 1604
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class CloningConsoleComponent : Component
	{
		// Token: 0x040014F0 RID: 5360
		public const string ScannerPort = "MedicalScannerSender";

		// Token: 0x040014F1 RID: 5361
		public const string PodPort = "CloningPodSender";

		// Token: 0x040014F2 RID: 5362
		[ViewVariables]
		public EntityUid? GeneticScanner;

		// Token: 0x040014F3 RID: 5363
		[ViewVariables]
		public EntityUid? CloningPod;

		// Token: 0x040014F4 RID: 5364
		[DataField("maxDistance", false, 1, false, false, null)]
		public float MaxDistance = 4f;

		// Token: 0x040014F5 RID: 5365
		public bool GeneticScannerInRange = true;

		// Token: 0x040014F6 RID: 5366
		public bool CloningPodInRange = true;
	}
}
