using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Anomaly.Components
{
	// Token: 0x020007CC RID: 1996
	[RegisterComponent]
	public sealed class GeneratingAnomalyGeneratorComponent : Component
	{
		// Token: 0x04001AE0 RID: 6880
		[DataField("endTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan EndTime = TimeSpan.Zero;

		// Token: 0x04001AE1 RID: 6881
		[Nullable(2)]
		public IPlayingAudioStream AudioStream;
	}
}
