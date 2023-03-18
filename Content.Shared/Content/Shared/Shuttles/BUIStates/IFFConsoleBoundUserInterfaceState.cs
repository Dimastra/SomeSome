using System;
using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates
{
	// Token: 0x020001D2 RID: 466
	[NetSerializable]
	[Serializable]
	public sealed class IFFConsoleBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x04000540 RID: 1344
		public IFFFlags AllowedFlags;

		// Token: 0x04000541 RID: 1345
		public IFFFlags Flags;
	}
}
