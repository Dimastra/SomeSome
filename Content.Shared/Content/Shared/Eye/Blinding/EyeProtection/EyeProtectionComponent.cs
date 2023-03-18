using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Eye.Blinding.EyeProtection
{
	// Token: 0x0200049E RID: 1182
	[RegisterComponent]
	public sealed class EyeProtectionComponent : Component
	{
		// Token: 0x04000D72 RID: 3442
		[DataField("protectionTime", false, 1, false, false, null)]
		public TimeSpan ProtectionTime = TimeSpan.FromSeconds(10.0);

		// Token: 0x04000D73 RID: 3443
		public bool IsActive;
	}
}
