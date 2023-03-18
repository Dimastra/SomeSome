using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Radio.Components
{
	// Token: 0x02000262 RID: 610
	[RegisterComponent]
	public sealed class WearingHeadsetComponent : Component
	{
		// Token: 0x0400078E RID: 1934
		[DataField("headset", false, 1, false, false, null)]
		public EntityUid Headset;
	}
}
