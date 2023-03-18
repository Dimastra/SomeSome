using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Flash.Components
{
	// Token: 0x020004FE RID: 1278
	[RegisterComponent]
	internal sealed class FlashOnTriggerComponent : Component
	{
		// Token: 0x040010B0 RID: 4272
		[DataField("range", false, 1, false, false, null)]
		internal float Range = 1f;

		// Token: 0x040010B1 RID: 4273
		[DataField("duration", false, 1, false, false, null)]
		internal float Duration = 8f;
	}
}
