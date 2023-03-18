using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Botany
{
	// Token: 0x020006F5 RID: 1781
	[RegisterComponent]
	public sealed class BotanySwabComponent : Component
	{
		// Token: 0x040016DF RID: 5855
		[DataField("swabDelay", false, 1, false, false, null)]
		public float SwabDelay = 2f;

		// Token: 0x040016E0 RID: 5856
		[Nullable(2)]
		public SeedData SeedData;
	}
}
