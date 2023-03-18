using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech
{
	// Token: 0x0200031A RID: 794
	[NetSerializable]
	[Serializable]
	public sealed class MechGrabberUiState : BoundUserInterfaceState
	{
		// Token: 0x0400090C RID: 2316
		[Nullable(1)]
		public List<EntityUid> Contents = new List<EntityUid>();

		// Token: 0x0400090D RID: 2317
		public int MaxContents;
	}
}
