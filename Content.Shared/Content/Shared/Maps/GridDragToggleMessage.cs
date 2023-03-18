using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Maps
{
	// Token: 0x0200033B RID: 827
	[NetSerializable]
	[Serializable]
	public sealed class GridDragToggleMessage : EntityEventArgs
	{
		// Token: 0x0400097C RID: 2428
		public bool Enabled;
	}
}
