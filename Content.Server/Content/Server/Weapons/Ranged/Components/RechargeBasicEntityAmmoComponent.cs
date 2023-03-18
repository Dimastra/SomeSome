using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Weapons.Ranged.Components
{
	// Token: 0x020000B6 RID: 182
	[RegisterComponent]
	public sealed class RechargeBasicEntityAmmoComponent : Component
	{
		// Token: 0x040001F9 RID: 505
		[ViewVariables]
		[DataField("minRechargeCooldown", false, 1, false, false, null)]
		public float MinRechargeCooldown = 30f;

		// Token: 0x040001FA RID: 506
		[ViewVariables]
		[DataField("maxRechargeCooldown", false, 1, false, false, null)]
		public float MaxRechargeCooldown = 45f;

		// Token: 0x040001FB RID: 507
		[Nullable(1)]
		[DataField("rechargeSound", false, 1, false, false, null)]
		public SoundSpecifier RechargeSound = new SoundPathSpecifier("/Audio/Magic/forcewall.ogg", null)
		{
			Params = AudioParams.Default.WithVolume(-5f)
		};

		// Token: 0x040001FC RID: 508
		[DataField("accumulatedFrametime", false, 1, false, false, null)]
		public float AccumulatedFrameTime;

		// Token: 0x040001FD RID: 509
		[ViewVariables]
		public float NextRechargeTime;
	}
}
