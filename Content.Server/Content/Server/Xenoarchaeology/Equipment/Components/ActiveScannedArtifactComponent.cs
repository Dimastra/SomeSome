using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.Equipment.Components
{
	// Token: 0x02000066 RID: 102
	[RegisterComponent]
	public sealed class ActiveScannedArtifactComponent : Component
	{
		// Token: 0x040000F6 RID: 246
		[ViewVariables]
		public EntityUid Scanner;

		// Token: 0x040000F7 RID: 247
		[Nullable(1)]
		public readonly SoundSpecifier ScanFailureSound = new SoundPathSpecifier("/Audio/Machines/custom_deny.ogg", null);
	}
}
