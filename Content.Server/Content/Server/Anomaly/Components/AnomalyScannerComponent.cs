using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Anomaly.Components
{
	// Token: 0x020007CA RID: 1994
	[RegisterComponent]
	public sealed class AnomalyScannerComponent : Component
	{
		// Token: 0x04001AD5 RID: 6869
		[ViewVariables]
		public EntityUid? ScannedAnomaly;

		// Token: 0x04001AD6 RID: 6870
		[DataField("scanDoAfterDuration", false, 1, false, false, null)]
		public float ScanDoAfterDuration = 5f;

		// Token: 0x04001AD7 RID: 6871
		[Nullable(2)]
		[DataField("completeSound", false, 1, false, false, null)]
		public SoundSpecifier CompleteSound = new SoundPathSpecifier("/Audio/Items/beep.ogg", null);
	}
}
