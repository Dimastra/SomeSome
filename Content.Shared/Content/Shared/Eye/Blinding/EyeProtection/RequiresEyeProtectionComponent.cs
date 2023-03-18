using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Eye.Blinding.EyeProtection
{
	// Token: 0x0200049F RID: 1183
	[RegisterComponent]
	public sealed class RequiresEyeProtectionComponent : Component
	{
		// Token: 0x04000D74 RID: 3444
		[DataField("statusEffectTime", false, 1, false, false, null)]
		public TimeSpan StatusEffectTime = TimeSpan.FromSeconds(10.0);

		// Token: 0x04000D75 RID: 3445
		[DataField("toggled", false, 1, false, false, null)]
		public bool Toggled;
	}
}
