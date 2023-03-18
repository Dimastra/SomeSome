using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Sound.Components
{
	// Token: 0x0200018D RID: 397
	[RegisterComponent]
	public sealed class EmitSoundOnUseComponent : BaseEmitSoundComponent
	{
		// Token: 0x0400045B RID: 1115
		[DataField("handle", false, 1, false, false, null)]
		public bool Handle = true;
	}
}
