using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Wieldable.Components
{
	// Token: 0x0200007E RID: 126
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(WieldableSystem)
	})]
	public sealed class WieldableComponent : Component
	{
		// Token: 0x0400014E RID: 334
		[DataField("wieldSound", false, 1, false, false, null)]
		public SoundSpecifier WieldSound = new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg", null);

		// Token: 0x0400014F RID: 335
		[DataField("unwieldSound", false, 1, false, false, null)]
		public SoundSpecifier UnwieldSound;

		// Token: 0x04000150 RID: 336
		[DataField("freeHandsRequired", false, 1, false, false, null)]
		public int FreeHandsRequired = 1;

		// Token: 0x04000151 RID: 337
		public bool Wielded;

		// Token: 0x04000152 RID: 338
		[Nullable(1)]
		[DataField("wieldedInhandPrefix", false, 1, false, false, null)]
		public string WieldedInhandPrefix = "wielded";

		// Token: 0x04000153 RID: 339
		public string OldInhandPrefix;

		// Token: 0x04000154 RID: 340
		[DataField("wieldTime", false, 1, false, false, null)]
		public float WieldTime = 1.5f;
	}
}
