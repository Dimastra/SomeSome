using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x02000613 RID: 1555
	[NetSerializable]
	[Serializable]
	public sealed class CartridgeComponentState : ComponentState
	{
		// Token: 0x040012C6 RID: 4806
		public InstallationStatus InstallationStatus;
	}
}
