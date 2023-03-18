using System;
using System.Runtime.CompilerServices;
using Content.Shared.Shuttles.Systems;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Shuttles.Components
{
	// Token: 0x02000205 RID: 517
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class FTLComponent : Component
	{
		// Token: 0x04000643 RID: 1603
		[ViewVariables]
		public FTLState State = FTLState.Available;

		// Token: 0x04000644 RID: 1604
		[ViewVariables]
		public float StartupTime;

		// Token: 0x04000645 RID: 1605
		[ViewVariables]
		public float TravelTime;

		// Token: 0x04000646 RID: 1606
		[ViewVariables]
		public float Accumulator;

		// Token: 0x04000647 RID: 1607
		[ViewVariables]
		[DataField("targetUid", false, 1, false, false, null)]
		public EntityUid? TargetUid;

		// Token: 0x04000648 RID: 1608
		[ViewVariables]
		[DataField("targetCoordinates", false, 1, false, false, null)]
		public EntityCoordinates TargetCoordinates;

		// Token: 0x04000649 RID: 1609
		[ViewVariables]
		[DataField("dock", false, 1, false, false, null)]
		public bool Dock;

		// Token: 0x0400064A RID: 1610
		[ViewVariables]
		[DataField("soundTravel", false, 1, false, false, null)]
		public SoundSpecifier TravelSound = new SoundPathSpecifier("/Audio/Effects/Shuttle/hyperspace_progress.ogg", null)
		{
			Params = AudioParams.Default.WithVolume(-3f).WithLoop(true)
		};

		// Token: 0x0400064B RID: 1611
		public IPlayingAudioStream TravelStream;
	}
}
