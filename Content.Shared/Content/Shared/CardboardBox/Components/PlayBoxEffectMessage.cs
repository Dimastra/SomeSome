using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CardboardBox.Components
{
	// Token: 0x02000639 RID: 1593
	[NetSerializable]
	[Serializable]
	public sealed class PlayBoxEffectMessage : EntityEventArgs
	{
		// Token: 0x0600132B RID: 4907 RVA: 0x0003FD6A File Offset: 0x0003DF6A
		public PlayBoxEffectMessage(EntityUid source, EntityUid mover)
		{
			this.Source = source;
			this.Mover = mover;
		}

		// Token: 0x0400132A RID: 4906
		public EntityUid Source;

		// Token: 0x0400132B RID: 4907
		public EntityUid Mover;
	}
}
