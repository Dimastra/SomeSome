using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Components
{
	// Token: 0x0200060B RID: 1547
	[RegisterComponent]
	public sealed class PartExchangerComponent : Component
	{
		// Token: 0x04001468 RID: 5224
		[DataField("exchangeDuration", false, 1, false, false, null)]
		public float ExchangeDuration = 3f;

		// Token: 0x04001469 RID: 5225
		[DataField("doDistanceCheck", false, 1, false, false, null)]
		public bool DoDistanceCheck = true;

		// Token: 0x0400146A RID: 5226
		[Nullable(1)]
		[DataField("exchangeSound", false, 1, false, false, null)]
		public SoundSpecifier ExchangeSound = new SoundPathSpecifier("/Audio/Items/rped.ogg", null);

		// Token: 0x0400146B RID: 5227
		[Nullable(2)]
		public IPlayingAudioStream AudioStream;
	}
}
