using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Speech.Components
{
	// Token: 0x020001C1 RID: 449
	[RegisterComponent]
	public sealed class ActiveListenerComponent : Component
	{
		// Token: 0x0400054F RID: 1359
		[DataField("range", false, 1, false, false, null)]
		public float Range = 10f;
	}
}
