using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Slippery
{
	// Token: 0x02000196 RID: 406
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class SlipperyComponent : Component
	{
		// Token: 0x0400046F RID: 1135
		[Nullable(1)]
		[DataField("slipSound", false, 1, false, false, null)]
		[Access]
		public SoundSpecifier SlipSound = new SoundPathSpecifier("/Audio/Effects/slip.ogg", null);

		// Token: 0x04000470 RID: 1136
		[ViewVariables]
		[DataField("paralyzeTime", false, 1, false, false, null)]
		[Access]
		public float ParalyzeTime = 3f;

		// Token: 0x04000471 RID: 1137
		[ViewVariables]
		[DataField("launchForwardsMultiplier", false, 1, false, false, null)]
		[Access]
		public float LaunchForwardsMultiplier = 1f;
	}
}
