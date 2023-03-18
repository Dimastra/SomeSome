using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Server.RCD.Components
{
	// Token: 0x0200024F RID: 591
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class RCDComponent : Component
	{
		// Token: 0x0400074D RID: 1869
		private const int DefaultAmmoCount = 5;

		// Token: 0x0400074E RID: 1870
		[ViewVariables]
		[DataField("maxAmmo", false, 1, false, false, null)]
		public int MaxAmmo = 5;

		// Token: 0x0400074F RID: 1871
		[ViewVariables]
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 2f;

		// Token: 0x04000750 RID: 1872
		[DataField("swapModeSound", false, 1, false, false, null)]
		public SoundSpecifier SwapModeSound = new SoundPathSpecifier("/Audio/Items/genhit.ogg", null);

		// Token: 0x04000751 RID: 1873
		[DataField("successSound", false, 1, false, false, null)]
		public SoundSpecifier SuccessSound = new SoundPathSpecifier("/Audio/Items/deconstruct.ogg", null);

		// Token: 0x04000752 RID: 1874
		[DataField("mode", false, 1, false, false, null)]
		public RcdMode Mode;

		// Token: 0x04000753 RID: 1875
		[ViewVariables]
		[DataField("ammo", false, 1, false, false, null)]
		public int CurrentAmmo = 5;

		// Token: 0x04000754 RID: 1876
		[Nullable(2)]
		public CancellationTokenSource CancelToken;

		// Token: 0x04000755 RID: 1877
		[DataField("autoRecharge", false, 1, false, false, null)]
		[ViewVariables]
		public bool AutoRecharge;

		// Token: 0x04000756 RID: 1878
		[DataField("rechargeDuration", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan RechargeDuration = TimeSpan.FromSeconds(20.0);

		// Token: 0x04000757 RID: 1879
		[DataField("nextChargeTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		[ViewVariables]
		public TimeSpan NextChargeTime = TimeSpan.FromSeconds(20.0);
	}
}
