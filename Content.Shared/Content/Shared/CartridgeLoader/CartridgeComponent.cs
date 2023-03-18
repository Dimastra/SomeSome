using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x02000612 RID: 1554
	[NetworkedComponent]
	[RegisterComponent]
	public sealed class CartridgeComponent : Component
	{
		// Token: 0x040012C3 RID: 4803
		[Nullable(1)]
		[DataField("programName", false, 1, true, false, null)]
		public string ProgramName = "default-program-name";

		// Token: 0x040012C4 RID: 4804
		[Nullable(2)]
		[DataField("icon", false, 1, false, false, null)]
		public SpriteSpecifier Icon;

		// Token: 0x040012C5 RID: 4805
		public InstallationStatus InstallationStatus;
	}
}
