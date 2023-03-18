using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Extinguisher
{
	// Token: 0x020004A0 RID: 1184
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedFireExtinguisherComponent : Component
	{
		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06000E5C RID: 3676 RVA: 0x0002E181 File Offset: 0x0002C381
		[DataField("safetySound", false, 1, false, false, null)]
		public SoundSpecifier SafetySound { get; } = new SoundPathSpecifier("/Audio/Machines/button.ogg", null);

		// Token: 0x04000D76 RID: 3446
		[DataField("refillSound", false, 1, false, false, null)]
		public SoundSpecifier RefillSound = new SoundPathSpecifier("/Audio/Effects/refill.ogg", null);

		// Token: 0x04000D77 RID: 3447
		[DataField("hasSafety", false, 1, false, false, null)]
		public bool HasSafety = true;

		// Token: 0x04000D78 RID: 3448
		[DataField("safety", false, 1, false, false, null)]
		public bool Safety = true;
	}
}
