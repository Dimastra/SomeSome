using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Wall
{
	// Token: 0x0200007D RID: 125
	[RegisterComponent]
	public sealed class WallMountComponent : Component
	{
		// Token: 0x040001A1 RID: 417
		[ViewVariables]
		[DataField("arc", false, 1, false, false, null)]
		public Angle Arc = new Angle(3.1415927410125732);

		// Token: 0x040001A2 RID: 418
		[ViewVariables]
		[DataField("direction", false, 1, false, false, null)]
		public Angle Direction = Angle.Zero;
	}
}
